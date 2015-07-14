
namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using RSA;

    using Stock;

    /// <summary>
    /// Handles communication between the client and the server.
    /// Makes sure that all traffic is encrypted.
    /// </summary>
    public class Communication
    {
        #region Constants and Fields

        private readonly AesManaged _aes = new AesManaged();

        private readonly Dictionary<String, int> _investmentData = new Dictionary<string, int>();

        private readonly Dictionary<String, List<StockDataPoint>> _publicStockData = new Dictionary<string, List<StockDataPoint>>();

        private readonly Uri _serverUri = new Uri("http://127.0.0.1:80/", UriKind.Absolute);

        private readonly UTF8Encoding _utf8 = new UTF8Encoding();

        private static Communication _instance;

        private Boolean _hasPublicKey = false;

        // Initialization vector for aes
        private byte[] _iv;

        private byte[] _keyFile;

        private String _lastErrorMessage = "";

        private byte[] _publicKey = null;

        private List<String> _stockGroupNames = new List<string>();

        private String _token = null;

        private Decimal _totalFunds = 0;

        private String _username = "";

        // number of failed tries to connet to the server
        // Used for user output only.
        private uint tries = 0;

        #endregion

        #region Constructors and Destructors

        private Communication()
        {
            CreateLocalKeyFiles();
            GetPublicKey();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current instance of the Communication class i.e. the login session.
        /// </summary>
        public static Communication Instance
        {
            get
            {
                if (_instance == null)
                    CreateNewInstance();

                return _instance;
            }
        }

        /// <summary>
        /// Returns true if the current session has a token.
        /// </summary>
        public Boolean HasToken
        {
            get
            {
                return _token != null;
            }
        }

        /// <summary>
        /// Returns a list of available stock group names.
        /// </summary>
        public List<String> StockGroups
        {
            get
            {
                return _stockGroupNames;
            }
        }

        /// <summary>
        /// Gets the total funds on the users investment account.
        /// </summary>
        public Decimal TotalFunds
        {
            get
            {
                return _totalFunds;
            }
        }

        /// <summary>
        /// The name of the current logged in user.
        /// </summary>
        public String Username
        {
            get
            {
                return _username;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Closes the current session if it exists and creates a new.
        /// </summary>
        public static void CreateNewInstance()
        {
            Contract.Ensures(!ReferenceEquals(Instance, null));
            Contract.Ensures(!Instance.HasToken);
            Contract.Ensures(Instance.StockGroups.Count == 0);
            Contract.Ensures(Instance.TotalFunds == 0);
            Contract.Ensures(Instance.Username.Equals(""));

            if (_instance != null && _instance.HasToken)
            {
                _instance.CloseConnection();
            }

            _instance = new Communication();
        }

        /// <summary>
        /// Close the connection to the server and reset all local private data.
        /// </summary>
        public void CloseConnection()
        {
            Contract.Ensures(!HasToken);
            Contract.Ensures(TotalFunds == 0);
            Contract.Ensures(Contract.ForAll(StockGroups, sg => Investments(sg) == 0));

            GetResponseFromServer(CloseConnectionResponse, KfEncryptAndBase64("logout"), _token);

            _token = null;
            _totalFunds = 0;
            _stockGroupNames.ForEach(sg => _investmentData[sg] = 0);
        }

        /// <summary>
        /// The percentage invested in a specific stock group.
        /// </summary>
        /// <param name="stockGroup">The stock group for which information is wanted.</param>
        /// <returns>The investment percentage for the specified stock group.</returns>
        public int Investments(String stockGroup)
        {
            Contract.Requires(!ReferenceEquals(stockGroup, null));
            Contract.Requires(Contract.Exists(StockGroups, name => name.Equals(stockGroup)));

            return _investmentData[stockGroup];
        }

        /// <summary>
        /// Sends a login request to the server with the specified username and password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="username">The username.</param>
        /// <returns>True if a key file exists for the specified username.</returns>
        public Boolean Login(String password, String username)
        {
            Contract.Requires(!ReferenceEquals(username, null));
            Contract.Requires(!ReferenceEquals(password, null));

            if (!_hasPublicKey) return false;

            Boolean canContinue = InitializeAES(username);

            if (!canContinue)
            {
                // username does not exist.
                return false;
            }

            CreateConnection(password, username, LoginResponse);

            _username = username;

            return true;
        }

        /// <summary>
        /// Sends a login request to the server with the current username and a specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        public void Relogin(String password)
        {
            Contract.Requires(!ReferenceEquals(password, null));

            CreateConnection(password, _username, ReloginResponse);
        }

        /// <summary>
        /// Gets a list of data points for a specific stock group.
        /// </summary>
        /// <param name="stockGroup">The stock group for which information is wanted.</param>
        /// <returns>A list of data points for the specified stock group.</returns>
        public List<StockDataPoint> StockGroupData(String stockGroup)
        {
            Contract.Requires(!ReferenceEquals(stockGroup, null));
            Contract.Requires(Contract.Exists(StockGroups, name => name.Equals(stockGroup)));

            return _publicStockData[stockGroup];
        }

        /// <summary>
        /// Saves a local copy of the investment data and sends a submit request to the server.
        /// </summary>
        /// <param name="data">The data to be submitted.</param>
        /// <returns>True if the data is correctly formatted.</returns>
        public Boolean SubmitData(Dictionary<String, int> data)
        {
            Contract.Requires(!ReferenceEquals(data, null));
            Contract.Requires(data.Count == StockGroups.Count);
            Contract.Requires(Contract.ForAll(StockGroups, sg => data.ContainsKey(sg)));
            Contract.Requires(StockGroups.Sum(sg => data[sg]) <= 100);
            Contract.Requires(Contract.ForAll(StockGroups, sg => data[sg] >= 0));
            Contract.Ensures(StockGroups.All(sg => data[sg] == Investments(sg)));

            if (!_stockGroupNames.All(sg => data.ContainsKey(sg)) || _stockGroupNames.Count != data.Count)
            {
                _lastErrorMessage = "error incomplete dataset";
                return false;
            }

            _stockGroupNames.ForEach(sg => _investmentData[sg] = data[sg]);

            SubmitDataToServer();

            return true;
        }

        #endregion

        #region Methods

        private void CloseConnectionResponse(String response)
        {
            if (response.Equals("goodbye") || response.Equals("error session expired"))
            {
                _lastErrorMessage = "Your session has been closed!";
            }
        }

        /// <summary>
        /// Establishes a connection to the server, logs in, and receives a token.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if the username is valid.</returns>
        private void CreateConnection(String password, String username, Action<String> loginAction)
        {
            Contract.Requires(!ReferenceEquals(password, null));
            Contract.Requires(!ReferenceEquals(username, null));

            SHA256Managed sha = new SHA256Managed();

            String pass64 = Convert.ToBase64String(sha.ComputeHash(_utf8.GetBytes(password)));

            String data = PpEncryptAndBase64("login;" + username + ";" + pass64 + ";" + Convert.ToBase64String(_iv));

            GetResponseFromServer(loginAction, data);
        }

        /// <summary>
        /// Create a key file in the local storage with the specified file name.
        /// </summary>
        /// <param name="filename">The name of the key file.</param>
        /// <param name="key">The key as a base64 string.</param>
        private void CreateKeyFile(String filename, String key)
        {
            Contract.Requires(!ReferenceEquals(key, null));

            byte[] keyFileContents = Convert.FromBase64String(key);
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

            if (!isf.FileExists(filename))
            {
                IsolatedStorageFileStream isfs = isf.CreateFile(filename);
                isfs.Write(keyFileContents, 0, keyFileContents.Length);
                isfs.Flush();
                isfs.Close();
            }
        }

        /// <summary>
        /// Creates three local key files used for testing.
        /// </summary>
        private void CreateLocalKeyFiles()
        {
            CreateKeyFile("Jannek.key", "Phe6lq4EzTuZhp5W8K2/Om+3TSVVF13MbosJFFeasDY=");
            CreateKeyFile("Mathias.key", "z9YoC7PHB9uu8nDclQUJbmdr5/ENRtgnuu0nIvu55P4=");
            CreateKeyFile("AverageJoe.key", "1meHGoWzhqivcnZjp4E3aYGareuzvymJ/QBRxp/k4PU=");
        }

        /// <summary>
        /// Retrieve the server's public key.
        /// </summary>
        private void GetPublicKey()
        {
            GetResponseFromServer(PublicKeyResponse, "getpublickey");
        }

        /// <summary>
        /// Sends a request to the server and executes a given delegate with the response as parameter.
        /// </summary>
        /// <param name="action">The delegate to be executed.</param>
        /// <param name="data">The request to be send to the server.</param>
        /// <param name="token">The current token if required for the request.</param>
        private void GetResponseFromServer(Action<String> action, String data, String token = "")
        {
            WebClient webClient = new WebClient();

            // webClient.Headers["token"] = token;
            // webClient.Headers["data"] = data;

            // Add delegate method to the event handler
            webClient.UploadStringCompleted += ReceiveData;

            // Send login request to the server
            webClient.UploadStringAsync(_serverUri, "POST", "data=" + data + "&token=" + token, action);

            Console.WriteLine("Waiting...");
        }

        /// <summary>
        /// Set the AES key and vector for the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>True if the username matches a existing key file.</returns>
        private Boolean InitializeAES(String username)
        {
            // Generate an initialization vector and save it.
            _aes.GenerateIV();
            _iv = _aes.IV;

            String filename = username + ".key";

            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

            // Read the local key file to _keyFile
            try
            {
                _keyFile = new byte[32];
                IsolatedStorageFileStream fs = isf.OpenFile(filename, FileMode.Open, FileAccess.Read);
                fs.Read(_keyFile, 0, _keyFile.Length);
            }
            catch (IsolatedStorageException e)
            {
                // Username does not exist.
                return false;
            }
            return true;
        }

        /// <summary>
        /// Decrypts a previously encrypted string using the private key file.
        /// </summary>
        /// <param name="encrypted">The byte array to be decrypted.</param>
        /// <returns>The decrypted byte array.</returns>
        private String KfDecrypt(String encrypted)
        {
            Contract.Requires(!ReferenceEquals(encrypted, null));

            ICryptoTransform aesDecryptor = _aes.CreateDecryptor(_keyFile, _iv);
            MemoryStream msDecrypt = new MemoryStream();
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesDecryptor, CryptoStreamMode.Write);

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encrypted);

                csDecrypt.Write(encryptedBytes, 0, encryptedBytes.Length);
                csDecrypt.FlushFinalBlock();
                byte[] data = msDecrypt.ToArray();
                return _utf8.GetString(data, 0, data.Length);
            }
            catch (Exception)
            {
                return "error malformed response";
            }
        }

        /// <summary>
        /// Decrypts a previously encrypted string using the private key file and splits the string.
        /// </summary>
        /// <param name="encrypted">The byte array to be decrypted.</param>
        /// <returns>The decrypted byte array as a string array.</returns>
        private String[] KfDecryptAndSplit(String encrypted)
        {
            Contract.Requires(!ReferenceEquals(encrypted, null));

            return KfDecrypt(encrypted).Split(';');
        }

        /// <summary>
        /// Encrypts a string using the private key file.
        /// </summary>
        /// <param name="s">The string to be encrypted.</param>
        /// <returns>The encrypted string as a byte array.</returns>
        private byte[] KfEncrypt(String s)
        {
            Contract.Requires(!ReferenceEquals(s, null));

            byte[] data = _utf8.GetBytes(s);

            ICryptoTransform aesEncryptor = _aes.CreateEncryptor(_keyFile, _iv);
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesEncryptor, CryptoStreamMode.Write);

            csEncrypt.Write(data, 0, data.Length);
            csEncrypt.FlushFinalBlock();

            return msEncrypt.ToArray();
        }

        /// <summary>
        /// Encrypts a string using the private key file and convert it to base64.
        /// </summary>
        /// <param name="s">The string to be encrypted.</param>
        /// <returns>The encrypted string in base64 format.</returns>
        private String KfEncryptAndBase64(String s)
        {
            Contract.Requires(!ReferenceEquals(s, null));

            return Convert.ToBase64String(KfEncrypt(s));
        }

        /// <summary>
        /// The delegate to be called when the server responds to a login request.
        /// </summary>
        /// <param name="response">The response frome the server.</param>
        private void LoginResponse(String response)
        {
            if (response.StartsWith("error"))
            {
                _token = null;
                _lastErrorMessage = response;

                if (_lastErrorMessage.Contains("password"))
                    LoginScreen.Instance.Error("Wrong password!\rPlease retype and try again.");
                else
                    LoginScreen.Instance.Error("Connection error!\rMake sure your phone is connected to the internet.");
                LoginScreen.Instance.MayLogin();
            }
            else
            {
                _token = KfDecrypt(response);

                LoginScreen.Instance.SetStatusMessage("Logged in successfully!\rFetching public stock data..");
                tries = 0;
                RequestPublicData();
            }
        }

        /// <summary>
        /// Encrypts a string with the server's public key and convert it to base64.
        /// </summary>
        /// <param name="s">The string to be encrypted.</param>
        /// <returns>The encrypted string in base64 format.</returns>
        private String PpEncryptAndBase64(String s)
        {
            Contract.Requires(!ReferenceEquals(s, null));
            RSACrypto rsa = new RSACrypto();
            rsa.ImportCspBlob(_publicKey);
            byte[] sEncrypted = rsa.Encrypt(_utf8.GetBytes(s));
            return Convert.ToBase64String(sEncrypted);
        }

        /// <summary>
        /// The delegate to be called when the server responds to a private investment data request.
        /// </summary>
        /// <param name="response">The response frome the server.</param>
        private void PrivateDataResponse(String response)
        {
            if (response.StartsWith("error"))
            {
                tries++;
                LoginScreen.Instance.Error("Download error!\r" +
                                           "Failed to download private investment data.\rIn " + tries + " tries.");
                RequestPrivateData();
                return;
            }

            String[] reply = KfDecryptAndSplit(response);

            // Parse and save investment data.
            int i = 0;
            _totalFunds = Convert.ToDecimal(reply[i++]);
            while (i < reply.Length)
            {
                _investmentData.Add(reply[i++], Convert.ToInt32(reply[i++]));
            }

            LoginScreen.Instance.SetStatusMessage("Logged in successfully!\rReceived public stock data!\rReceived private investment data!");
            LoginScreen.Instance.LoginComplete();
        }

        /// <summary>
        /// The delegate to be called when the server responds to a public stock data request.
        /// </summary>
        /// <param name="response">The response frome the server.</param>
        private void PublicDataResponse(String response)
        {
            if (response.StartsWith("error"))
            {
                tries++;
                LoginScreen.Instance.Error("Download error!\r" + 
                    "Failed to download public stock data.In " + tries + " tries.");
                RequestPublicData();
                return;
            }

            String[] reply = KfDecryptAndSplit(response);

            // Parse and save stock group data.
            int i = 0;

            while (i < reply.Length)
            {
                List<StockDataPoint> dataList = new List<StockDataPoint>();
                String stockName = reply[i++];
                int numberOfCoords = Convert.ToInt32(reply[i++]);
                for (int j = 0; j < numberOfCoords; j++)
                {
                    dataList.Add(new StockDataPoint(Convert.ToUInt32(reply[i++]), Convert.ToInt64(reply[i++])));
                }

                _stockGroupNames.Add(stockName);
                _publicStockData.Add(stockName, dataList);
            }

            LoginScreen.Instance.SetStatusMessage("Logged in successfully!\rReceived public stock data!\rFetching private investment data..");
            tries = 0;
            RequestPrivateData();
        }

        /// <summary>
        /// The delegate to be called when the server responds to a public key request.
        /// </summary>
        /// <param name="response">The response frome the server.</param>
        private void PublicKeyResponse(String response)
        {
            if (response.StartsWith("error"))
            {
                tries++;
                _hasPublicKey = false;
                LoginScreen.Instance.Error("Failed to fetch public key\rAfter " + tries + " tries");
                GetPublicKey();
            }
            else
            {
                _publicKey = Convert.FromBase64String(response);
                _hasPublicKey = true;
                LoginScreen.Instance.MayLogin();
            }
        }
        
        /// <summary>
        /// The delegate to be called by any server request.
        /// </summary>
        /// <param name="args">Contains the response from the server and the delegate to call.</param>
        private void ReceiveData(object o, UploadStringCompletedEventArgs args)
        {
            Action<String> action = args.UserState as Action<String>;
            if (args.Error != null)
            {
                _lastErrorMessage = "error connection " + args.Error.ToString();
                action(_lastErrorMessage);
            }
            else if (String.IsNullOrEmpty(args.Result))
            {
                _lastErrorMessage = "error empty string";
                action(_lastErrorMessage);
            }
            else if (args.Result.Equals("error session expired"))
            {
                _token = null;
                _lastErrorMessage = args.Result;
                action(_lastErrorMessage);
            }
            else if (args.Result.StartsWith("error"))
            {
                _lastErrorMessage = args.Result;
                action(_lastErrorMessage);
            }
            else
            {
                _lastErrorMessage = "";
                action(args.Result);
            }
        }

        /// <summary>
        /// The delegate to be called when the server responds to a relogin request.
        /// </summary>
        /// <param name="response">The response frome the server.</param>
        private void ReloginResponse(String response)
        {
            if (response.StartsWith("error"))
            {
                _token = null;
                _lastErrorMessage = response;
                if (response.Contains("password"))
                    ReloginScreen.Instance.Error("Wrong password!\rPlease retype and try again.");
                else
                    ReloginScreen.Instance.Error("Connection error!\rMake sure your phone is connected to the internet.");
            }
            else
            {
                _token = KfDecrypt(response);

                ReloginScreen.Instance.ReloginComplete();
                SubmitDataToServer();
            }
        }

        /// <summary>
        /// Request the private investment data from the server.
        /// </summary>
        private void RequestPrivateData()
        {
            GetResponseFromServer(PrivateDataResponse, KfEncryptAndBase64("getinvestmentdata"), _token);
        }

        /// <summary>
        /// Requests the public stock data from the server.
        /// </summary>
        private void RequestPublicData()
        {
            GetResponseFromServer(PublicDataResponse, KfEncryptAndBase64("getstockdata"), _token);
        }

        /// <summary>
        /// Sends the local investment data to the server.
        /// </summary>
        private void SubmitDataToServer()
        {
            // Build the data string
            StringBuilder sbData = new StringBuilder();
            sbData.Append("submitdata");

            // Append all pairs to the request string
            foreach (KeyValuePair<string, int> pair in _investmentData)
            {
                sbData.AppendFormat(";{0};{1}", pair.Key, pair.Value);
            }

            GetResponseFromServer(SubmitResponse, KfEncryptAndBase64(sbData.ToString()), _token);
        }

        /// <summary>
        /// The delegate to be called when the server responds to a submit request.
        /// </summary>
        /// <param name="response">The response frome the server.</param>
        private void SubmitResponse(String response)
        {
            Contract.Requires(!ReferenceEquals(response, null));

            InvestmentScreen.Instance.DataHasBeenSubmitted = KfDecrypt(response).Equals("done");

            if (response.Contains("expired"))
            {
                InvestmentScreen.Instance.ReLogin();
            }
            else if (response.StartsWith("error"))
            {
                InvestmentScreen.Instance.ConnectionError();
            }
            InvestmentScreen.Instance.UpdateSynchronizedText();
        }

        #endregion
    }
}
