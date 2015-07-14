namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Xml;

    using Logger;

    using Server.StockData;
    using Server.UserData;

    using Stock;

    /// <summary>
    /// Interface for the client to fetch and submit private user data and public stock data.
    /// </summary>
    public class ServerClass
    {
        #region Constants and Fields

        private Dictionary<String, User> activeSessions;

        private SqlCommand command;

        private SqlConnection connection;

        private RequestManager requestManager;

        private List<StockGroup> stockGroups = new List<StockGroup>();

        #endregion

        #region Constructors and Destructors

        public ServerClass(String predictorName, String pathToFixml)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(predictorName));
            Contract.Requires(!String.IsNullOrWhiteSpace(pathToFixml));
            Contract.Requires(File.Exists(pathToFixml));

            UpdateStockData(predictorName, pathToFixml);

            requestManager = new RequestManager(this);
            activeSessions = new Dictionary<string, User>();

            CreateDatabaseConnection();
            if (connection.State != ConnectionState.Open)
            {
                Log.Write("Database connection could not be created.");
                return;
            }
            connection.Close();

            ThreadPool.QueueUserWorkItem(RemoveExpiredUsersLoop);

            Log.Write("Server Class created.");
            Log.Write("Connection to database: " + connection.State.ToString());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of all stock groups.
        /// </summary>
        public List<StockGroup> StockGroups
        {
            get
            {
                return stockGroups;
            }
        }

        #endregion

        #region Public Methods

        public static void Main(String[] args)
        {
            Contract.Requires(!ReferenceEquals(args, null));
            Contract.Requires(args.All(s => !String.IsNullOrWhiteSpace(s)));
            Contract.Requires(args.Length >= 2);
            Contract.Requires(File.Exists(args[1]));

            if (args.Length < 2)
            {
                Console.WriteLine("To start the PocketInvester Server use:");
                Console.WriteLine("./Server.exe <predictor name> <path to FIXML>");
                return;
            }
            Log.Write("PocketInvestor starting.");
            ServerClass server = new ServerClass(args[0], args[1]);

            if (args.Any(a => a.Equals("-genkeys")))
                server.GenerateKeyFiles();

            server.requestManager.StartServer();
        }

        /// <summary>
        /// Returns the user data associated with the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The user</returns>
        public User GetPrivateUserData(String token)
        {
            Contract.Requires(IsTokenValid(token));
            Contract.Ensures(!ReferenceEquals(Contract.Result<User>(), null));

            lock (activeSessions)
            {
                return activeSessions[token];
            }
        }

        /// <summary>
        /// Returns true if the specified token does not expire within five minutes.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>True if the token is valid.</returns>
        [Pure]
        public bool IsTokenValid(String token)
        {
            Contract.Requires(!ReferenceEquals(token, null));
            lock (activeSessions)
            {
                if (!activeSessions.ContainsKey(token)) return false;
                User user = activeSessions[token];
                if (user.VerifyAndExtendLoginSession()) return true;
                Log.Write("Session expired for: " + activeSessions[token].Name);
                activeSessions.Remove(token);
            }
            return false;
        }

        /// <summary>
        /// Atempts to creates a new user session with the specified parameters and returns a token.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="iv">The initialization vector for aes.</param>
        /// <returns>The associated token if successful.</returns>
        public String Login(String username, String password, byte[] iv)
        {
            Contract.Requires(!ReferenceEquals(username, null));
            Contract.Requires(!ReferenceEquals(password, null));
            Contract.Requires(!ReferenceEquals(iv, null));
            Contract.Ensures(!Contract.Result<String>().Equals("") == IsTokenValid(Contract.Result<String>()));

            if (username.Contains("'") || password.Contains("'")) return "";

            Log.Write("User login: " + username + " pass: " + password);

            int id;
            decimal funds;
            byte[] key;
            List<InvestmentData> investments = new List<InvestmentData>();

            lock (command)
            {
                CreateDatabaseConnection();

                command.CommandText = "select Id, Funds, KeyFile from piUser where Name='" + username
                                      + "' and Password='" + password + "'";
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
                if (!reader.HasRows) return "";
                reader.Read();

                id = reader.GetInt32(0);
                funds = reader.GetDecimal(1);
                key = new byte[32];
                reader.GetBytes(2, 0, key, 0, 32);
                reader.Close();

                Log.Write("User found in db: " + id);

                if (id <= 0) return "";

                command.CommandText = "select StockGroup, Value from Investment where [User]=" + id;
                reader = command.ExecuteReader();
                Log.Write("User has investments: " + reader.HasRows);

                while (reader.Read())
                {
                    investments.Add(new InvestmentData(reader.GetString(0), (uint)reader.GetInt32(1)));
                    Log.Write("Investments: " + investments.Count);
                }

                reader.Close();

                connection.Close();
            }

            User user = new User(username, id, funds, investments, key, iv, this);

            String token = "";

            lock (activeSessions)
            {
                do
                {
                    Random rand = new Random();
                    byte[] randomBytes = new byte[16];
                    rand.NextBytes(randomBytes);
                    token = Convert.ToBase64String(randomBytes);
                }
                while (activeSessions.ContainsKey(token));

                activeSessions.Add(token, user);
            }

            Log.Write(" > Token: " + token);
            Log.Write(" > KF: " + Convert.ToBase64String(user.KeyFile));
            Log.Write(" > IV: " + Convert.ToBase64String(user.IV));

            return token;
        }

        /// <summary>
        /// Closes the session associated with the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        public void Logout(String token)
        {
            Contract.Requires(!ReferenceEquals(token, null));
            Contract.Ensures(!IsTokenValid(token));
            lock (activeSessions)
            {
                if (activeSessions.ContainsKey(token))
                    activeSessions.Remove(token);
            }
        }

        /// <summary>
        /// Saves the user investment data in the database.
        /// </summary>
        /// <param name="user">User whose data should be saved.</param>
        public void SaveUserInvestmentData(User user)
        {
            Contract.Requires(!ReferenceEquals(user, null));
            lock (command)
            {
                CreateDatabaseConnection();

                foreach (InvestmentData investmentData in user.InvestmentData)
                {
                    command.CommandText = "update Investment set Value=" + investmentData.InvestmentPercentage
                                          + " where StockGroup='" + investmentData.StockGroup + "' and [User]="
                                          + user.Id;
                    command.ExecuteNonQuery();
                }
                Log.Write("Userdata has been updated!");

                connection.Close();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a key file to the specified user in the data base.
        /// </summary>
        /// <param name="name">Specified username.</param>
        /// <param name="keyFile">Key file to add.</param>
        private void AddKeyFileToUserInDatabase(String name, byte[] keyFile)
        {
            Contract.Requires(!ReferenceEquals(keyFile, null));
            CreateDatabaseConnection();
            using (SqlCommand keyCommand = new SqlCommand("update piUser set KeyFile=@BinaryKeyFile where Name=@UserName", connection))
            {
                keyCommand.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = name;
                keyCommand.Parameters.Add("@BinaryKeyFile", SqlDbType.Binary, 32).Value = keyFile;
                int affected = keyCommand.ExecuteNonQuery();
                Log.Write("Updated key for " + name + " : " + affected);
                Log.Write(" > KF: " + Convert.ToBase64String(keyFile));
                keyCommand.Parameters.Clear();
                keyCommand.Dispose();
            }
            connection.Close();
        }

        /// <summary>
        /// Creates a connection to the database if it is not allready open.
        /// </summary>
        private void CreateDatabaseConnection()
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                String connectionString =
                    "Data Source=kenc.dk;Persist Security Info=True;User ID=PocketInvestor;Password=Pass!word#";
                connection = new SqlConnection(connectionString);
                command = connection.CreateCommand();
                connection.Open();
            }
        }

        /// <summary>
        /// Generates and add key files for all of the three users to the database.
        /// </summary>
        private void GenerateKeyFiles()
        {
            byte[] jannek = new byte[32];
            byte[] mathias = new byte[32];
            byte[] averageJoe = new byte[32];

            Random random = new Random(42);

            random.NextBytes(jannek);
            random.NextBytes(mathias);
            random.NextBytes(averageJoe);

            AddKeyFileToUserInDatabase("Jannek", jannek);
            AddKeyFileToUserInDatabase("Mathias", mathias);
            AddKeyFileToUserInDatabase("AverageJoe", averageJoe);
        }

        /// <summary>
        /// A loop running removing expired login sessions every 15 minute.
        /// </summary>
        private void RemoveExpiredUsersLoop(Object o)
        {
            while (true)
            {
                Log.Write("Starting the \"Remove expired clients loop\" to run every 15 minutes");
                // sleep for 15 minutes
                Thread.Sleep(15 * 60000);
                DateTime now = DateTime.Now;
                List<String> toRemove = new List<string>();
                lock (activeSessions)
                {
                    foreach (KeyValuePair<string, User> pair in activeSessions)
                    {
                        if (pair.Value.ExpirationTime.CompareTo(now) < 0)
                        {
                            toRemove.Add(pair.Key);
                        }
                    }
                    toRemove.ForEach(token => activeSessions.Remove(token));
                }
                Log.Write(toRemove.Count + " users removed.");
                Log.Write("Current users: " + activeSessions.Count);
            }
        }

        /// <summary>
        /// Reads the stock data from a FIXML file, predicts its trends and save it in the memory.
        /// </summary>
        /// <param name="predictorName">Name of the prediction plugin.</param>
        /// <param name="xmlFilePath">Path to the FIXML file.</param>
        private void UpdateStockData(String predictorName, String xmlFilePath)
        {
            XmlTextReader reader = new XmlTextReader(xmlFilePath);
            reader.WhitespaceHandling = WhitespaceHandling.None;

            String stockName = String.Empty;
            String stockPriceString = String.Empty;
            String stockTransactionTime = String.Empty;

            while (reader.Read())
            {
                if (reader.Name.Equals("Order") && reader.NodeType == XmlNodeType.Element)
                {
                    #region XML parsing
                    // Get the attributes we want from <Order>
                    stockTransactionTime = reader.GetAttribute("TransactTm");
                    stockPriceString = reader.GetAttribute("Px");

                    // Move back to <Order> node
                    reader.MoveToElement();

                    // Move to <Hdr> node and skip it's children
                    reader.Read();
                    reader.Skip();

                    // We are in <Instrmt> node. Get the attribute we want
                    stockName = reader.GetAttribute("Sym");
                    reader.MoveToElement();
                    #endregion XML parsing

                    #region Data manipulation
                    // Convert stockPriceString to uint - discard decimals
                    Debug.Assert(stockPriceString != null);
                    uint stockPrice = Convert.ToUInt32(stockPriceString.Split(',')[0]);

                    // Convert stockTransactionTime to long
                    // Format of stockTransactionTime: YYYY-MM-DDTHH:MM:SS
                    Debug.Assert(stockTransactionTime != null);
                    int year = Convert.ToInt32(stockTransactionTime.Substring(0, 4));
                    int month = Convert.ToInt32(stockTransactionTime.Substring(5, 2));
                    int day = Convert.ToInt32(stockTransactionTime.Substring(8, 2));
                    int hour = Convert.ToInt32(stockTransactionTime.Substring(11, 2));
                    int minute = Convert.ToInt32(stockTransactionTime.Substring(14, 2));
                    int second = Convert.ToInt32(stockTransactionTime.Substring(17, 2));

                    // Get milliseconds since unixStartDate
                    //DateTime unixStartDate = new DateTime(1970, 1, 1);
                    DateTime stockTime = new DateTime(year, month, day, hour, minute, second);
                    //long ticksSinceUnixStartDate = stockTime.Ticks - unixStartDate.Ticks;
                    long stockTimeStampMillis = stockTime.Ticks / 10000000; // 10.000.000 ticks = 1 second

                    // Save the data
                    int i = -1;
                    i = stockGroups.FindIndex(stockGroup => stockGroup.Name.Equals(stockName));
                    if (i != -1)
                    {
                        // stockName already exist in stockGroups
                        // Add new values to stockName's data points
                        List<StockDataPoint> dataPoints = stockGroups[i].DataPoints;
                        dataPoints.Add(new StockDataPoint(stockPrice, stockTimeStampMillis));
                        stockGroups[i].DataPoints = dataPoints;
                    }
                    else
                    {
                        // stockName does not exist in stockGroups
                        // Create new list of data points
                        List<StockDataPoint> dataPoints = new List<StockDataPoint>();
                        // Add the initial stock price to the list
                        dataPoints.Add(new StockDataPoint(stockPrice, stockTimeStampMillis));
                        // Add the new stockName to stockGroups
                        stockGroups.Add(new StockGroup(stockName, dataPoints));
                    }
                    #endregion Data manipulation
                }
            }

            foreach (StockGroup stockGroup in stockGroups)
            {
                Log.Write("Stock name: " + stockGroup.Name);
            }

            Log.Write("Searching for " + predictorName + ".dll in plugins folder");
            Assembly predictionAssembly = Assembly.LoadFrom("./plugins/" + predictorName + ".dll");
            StockPrediction predictor = null;

            foreach (Type type in predictionAssembly.GetTypes())
            {
                Log.Write(" > Found: " + type.Name);
                if (type.IsClass && type.GetInterface("StockPrediction") != null)
                {
                    Log.Write("a Stock predictor was found");
                    predictor = Activator.CreateInstance(type, new object[0]) as StockPrediction;
                }
            }

            if (predictor == null)
                Log.Write("No stock predictor was found in the dll file");

            // Call prediction on all stock groups
            foreach (StockGroup stockGroup in stockGroups)
            {
                predictor.Predict(stockGroup);
            }

            Log.Write("Current time in seconds: " + (DateTime.Now - new DateTime()).TotalSeconds);
        }

        #endregion
    }
}
