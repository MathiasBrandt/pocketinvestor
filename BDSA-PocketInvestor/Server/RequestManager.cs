namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using Logger;

    using RSA;

    using Server.UserData;

    using Stock;

    /// <summary>
    /// Translates http streams into method calls and passes them on to the ServerClass.
    /// Translates the response back into an http reply.
    /// </summary>
    public class RequestManager
    {
        #region Constants and Fields

        private AesManaged aes;

        private bool alive;

        private HttpListener listener;

        private byte[] publicKey;

        private RSACrypto rsa;

        private ServerClass serverClass;

        private UTF8Encoding utf8;

        private EventWaitHandle waitForKeyGeneration;

        #endregion

        #region Constructors and Destructors

        public RequestManager(ServerClass serverClass)
        {
            this.serverClass = serverClass;

            utf8 = new UTF8Encoding();

            Log.Write("Starting to generate public/private key pair.");
            waitForKeyGeneration = new EventWaitHandle(false, EventResetMode.ManualReset);
            rsa = new RSACrypto();
            rsa.OnKeysGenerated += new RSACrypto.KeysGenerated(KeyGenerated);
            rsa.GenerateKeys(2048);

            aes = new AesManaged();

            listener = new HttpListener();
            listener.Prefixes.Add("http://*:80/");
            alive = false;

            Log.Write("Request manager created, and ready to start server.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the server as soon as the public key is generated.
        /// </summary>
        public void StartServer()
        {
            Log.Write("Waiting for RSA key to be generated..");
            waitForKeyGeneration.WaitOne();
            Log.Write("Starting server.");
            alive = true;
            listener.Start();
            new Thread(ServerLoop).Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Decrypts and splits the specified string with the specified users key file and initialization vector.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="base64">The data parsed as a base64 string.</param>
        /// <returns>The decrypted string split on ";" into an array.</returns>
        private String[] DecryptAndSplit(User user, String base64)
        {
            Contract.Requires(!ReferenceEquals(base64, null));

            byte[] data = Convert.FromBase64String(base64);
            ICryptoTransform transform = aes.CreateDecryptor(user.KeyFile, user.IV);

            String deData;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                deData = utf8.GetString(memoryStream.ToArray());
            }
            catch (Exception)
            {
                deData = "decrypt error";
            }

            Log.Write(" > in: " + deData);
            return deData.Split(';');
        }

        /// <summary>
        /// Encrypts the specified base64 string and converts it to a base64 string, using the users key file and initialization vector.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="data">The data.</param>
        /// <returns>The encrypted base64 string.</returns>
        private byte[] EncryptAndBase64(User user, String data)
        {
            Contract.Requires(!ReferenceEquals(data, null));

            return EncryptAndBase64(user, utf8.GetBytes(data));
        }

        /// <summary>
        /// Encrypts and converts the specified data to a base64 string, using the users key file and initialization vector.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="data">The data.</param>
        /// <returns>The encrypted base64 string.</returns>
        private byte[] EncryptAndBase64(User user, byte[] data)
        {
            ICryptoTransform transform = aes.CreateEncryptor(user.KeyFile, user.IV);

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();

            return utf8.GetBytes(Convert.ToBase64String(memoryStream.ToArray()));
        }
        
        /// <summary>
        /// Resonds to the client whith a specified error message.
        /// </summary>
        /// <param name="context">The http client.</param>
        /// <param name="message">THe message.</param>
        private void Error(HttpListenerContext context, String message)
        {
            context.Response.Close(utf8.GetBytes("error " + message), true);
            Log.Write(" > Error " + message);
        }

        /// <summary>
        /// Converts the specified user's private investment data to a string parsable by the client.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Investment data as a string.</returns>
        private String GetInvestmentDataAsString(User user)
        {
            StringBuilder output = new StringBuilder();
            output.Append(user.Funds);
            user.InvestmentData.ForEach(id => output.Append(";" + id.StockGroup + ";" + id.InvestmentPercentage));

            return output.ToString();
        }

        /// <summary>
        /// Converts the public stock data to a string parsable by the client.
        /// </summary>
        /// <returns>Stock data as a string.</returns>
        private String GetStockDataAsString()
        {
            List<StockGroup> stocks = serverClass.StockGroups;

            StringBuilder output = new StringBuilder();

            foreach (StockGroup stock in stocks)
            {
                output.Append(stock.Name + ";" + stock.DataPoints.Count + ";");
                stock.DataPoints.ForEach(point => output.Append(point.Value + ";" + point.TimeStamp + ";"));
            }

            output.Remove(output.Length - 1, 1);

            return output.ToString();
        }

        /// <summary>
        /// Delegate to be called when the public RSA key has been generated, notifying that the server can start listening.
        /// </summary>
        private void KeyGenerated(Object o)
        {
            Log.Write("Key was created: " + !ReferenceEquals(rsa.ExportCspBlob(false), null));
            publicKey = utf8.GetBytes(Convert.ToBase64String(rsa.ExportCspBlob(false)));
            Log.Write("Max RSA Message length: " + rsa.MaxMessageLength);
            waitForKeyGeneration.Set();
        }

        /// <summary>
        /// Proccesses and responds to a http request from a client.
        /// </summary>
        /// <param name="hlc">The http request.</param>
        private void ProcessRequest(Object hlc)
        {
            Contract.Requires(!ReferenceEquals(hlc as HttpListenerContext, null));
            HttpListenerContext context = hlc as HttpListenerContext;
            Log.Write("Request received");
            Log.Write(" > From " + context.Request.RemoteEndPoint.ToString());

            StreamReader reader = new StreamReader(context.Request.InputStream);
            String input = reader.ReadToEnd();
            Log.Write(" > Input " + input);

            if (String.IsNullOrWhiteSpace(input))
            {
                Error(context, "nothing received");
                return;
            }

            String[] split = input.Split('&');
            String data = "";
            String token = "";
            if (split.Any(s => s.StartsWith("data=")))
                data = split.First(s => s.StartsWith("data=")).Substring(5);
            if (split.Any(s => s.StartsWith("token=")))
                token = split.First(s => s.StartsWith("token=")).Substring(6);

            if (String.IsNullOrEmpty(data))
            {
                // All valid requests have a data header
                Error(context, "no data received");
                return;
            }

            //Log.Write(data.Substring(0, Math.Min(data.Length, 50)));

            if (String.IsNullOrEmpty(token))
            {
                // This must be a login or public key request
                if (data.Equals("getpublickey"))
                {
                    // get public key is the only unecrypted request
                    context.Response.Close(publicKey, true);
                }
                else
                {
                    // login is the only encrypted tokenless request
                    String deData;
                    try
                    {
                        deData = utf8.GetString(rsa.Decrypt(Convert.FromBase64String(data)));
                    }
                    catch (Exception)
                    {
                        deData = "crypto error";
                    }
                    String[] array = deData.Split(';');
                    if (array.Length != 4 || !array[0].Equals("login"))
                    {
                        // doesn't match encryption request
                        Error(context, "malformed request");
                        Log.Write(" > Decrypted " + deData);
                        return;
                    }

                    byte[] iv = Convert.FromBase64String(array[3]);
                    if (iv.Length * 8 != aes.BlockSize)
                    {
                        // Vector must be same size as the value of BlockSize
                        Error(context, "malformed request");
                        return;
                    }

                    String sessionToken = serverClass.Login(array[1], array[2], iv);

                    if (sessionToken.Equals(""))
                    {
                        // User not found
                        Error(context, "wrong username or password");
                        return;
                    }

                    User user = serverClass.GetPrivateUserData(sessionToken);

                    // return session token
                    context.Response.Close(EncryptAndBase64(user, sessionToken), true);
                }

            }
            else
            {
                if (!serverClass.IsTokenValid(token))
                {
                    Error(context, "session expired");
                    return;
                }

                User user = serverClass.GetPrivateUserData(token);
                String[] array = DecryptAndSplit(user, data);

                switch (array[0])
                {
                    case "getstockdata":
                        String stockData = GetStockDataAsString();
                        Log.Write(" > out: " + stockData);
                        context.Response.Close(EncryptAndBase64(user, stockData), true);
                        break;
                    case "getinvestmentdata":
                        String investmentData = GetInvestmentDataAsString(user);
                        Log.Write(" > out: " + investmentData);
                        context.Response.Close(EncryptAndBase64(user, investmentData), true);
                        break;
                    case "logout":
                        serverClass.Logout(token);
                        context.Response.Close(EncryptAndBase64(user, "goodbye"), true);
                        break;
                    case "submitdata":
                        if (SubmitData(user, array))
                            context.Response.Close(EncryptAndBase64(user, "done"), true);
                        else
                            Error(context, "malformed data");
                        break;
                    case "decrypt error":
                    default:
                        Error(context, "malformed request");
                        break;
                }
                Log.Write(" > " + array[0]);
            }
        }

        /// <summary>
        /// A simple server loop to receive incomming requests and parse them on to a new thread.
        /// </summary>
        private void ServerLoop()
        {
            while (alive)
            {
                Log.Write("Waiting for incomming connections.");
                HttpListenerContext hlc = listener.GetContext();
                ThreadPool.QueueUserWorkItem(ProcessRequest, hlc);
            }
        }

        /// <summary>
        /// Saves the private investment data to the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="data">The data.</param>
        /// <returns>True if the data is valid and submitted.</returns>
        private bool SubmitData(User user, String[] data)
        {
            List<InvestmentData> list = new List<InvestmentData>();
            try
            {
                for (int i = 1; i < data.Length - 1; i += 2)
                {
                    list.Add(new InvestmentData(data[i], Convert.ToUInt32(data[i + 1])));
                }
            }
            catch (FormatException)
            {
                Log.Write(" > data: FormatException");
                return false;
            }
            catch (OverflowException)
            {
                Log.Write(" > data: OverflowException");
                return false;
            }
            Log.Write(" > data: count: " + user.InvestmentData.Count + " vs: " + list.Count);
            if (user.InvestmentData.Count != list.Count) return false;
            if (!user.InvestmentData.TrueForAll(uid =>
                list.Count(id => id.StockGroup.Equals(uid.StockGroup)) == 1)) return false;
            Log.Write(" > data match!");

            user.SaveData(list);
            return true;
        }

        #endregion
    }
}
