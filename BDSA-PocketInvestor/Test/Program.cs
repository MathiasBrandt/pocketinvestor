
namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Security.Cryptography;
    using System.Threading;

    class Program
    {
        public static void Main(String[] args)
        {
            Program p = new Program();
            String response = p.GetResponseFromServer(args[0]);
            Console.WriteLine(response);
        }

        private WebClient _webClient = new WebClient();

        public String responseFromServer = "";

        public Program()
        {
            // Add delegate method to the event handler
            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ReceiveData);

        }

        public String GetResponseFromServer(String data, String token = "")
        {
            lock (_webClient)
            {
                responseFromServer = "";
                _webClient.Headers.Clear();

                _webClient.Headers["token"] = token;
                _webClient.Headers["data"] = data;

                // Send login request to the server
                _webClient.DownloadStringAsync(new Uri("http://127.0.0.1:80/", UriKind.Absolute));

                Console.WriteLine("Waiting...");

                // Wait for ReceiveToken to complete
                while (responseFromServer.Equals(""))
                {
                    Thread.Yield();
                }
                return responseFromServer;
            }
        }

        /// <summary>
        /// Delegate method to receive data from the server.
        /// </summary>
        private void ReceiveData(object o, DownloadStringCompletedEventArgs args)
        {
            if (args.Error != null)
            {
                responseFromServer = "error connection " + args.Error.ToString();
                return;
            }
            if (String.IsNullOrEmpty(args.Result))
            {
                responseFromServer = "error empty string";
                return;
            }
            responseFromServer = args.Result;
        }
    }
}
