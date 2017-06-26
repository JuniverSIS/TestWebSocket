using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebSocketClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var test11 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 124);

            var test1 = "ws://session1-dev.rhymetree.net:443";
            var test2 = "192.168.0.161:6124";

            var testUri1 = new Uri(test1);
            var testUri2 = new Uri(string.Format("tcp://{0}", test2));
            var testUri3 = new Uri(string.Format("tcps://{0}", test2));

            var testUri4 = new Uri("tcp://");


            




            if (args.Length != 1)
            {
                PrintUsage();
                return;
            }

            var uri = UriTryParse(args[0]);
            if (uri == null)
            {
                PrintUsage();
                return;
            }

            var main = new Main(uri);
            main.MainAsync().Wait();

            Logger.Info("");
            Logger.Info("press any key...");
            Console.ReadLine();
        }

        private static void PrintUsage()
        {
            Logger.Info("");
            Logger.Warn("invalid_usage");
            Logger.Info("");
            Logger.Info("TestWebSocketClient.exe  ws://52.69.235.142:80");
            Logger.Info("TestWebSocketClient.exe wss://52.69.235.142:443");

            Logger.Info("TestWebSocketClient.exe  ws://session2-stage.rhymetree.net:16124");
            Logger.Info("TestWebSocketClient.exe wss://session2-stage.rhymetree.net:443");

            Logger.Info("TestWebSocketClient.exe  ws://gs-stage-ap-elb-client-test-1560911213.ap-northeast-1.elb.amazonaws.com:16442");
            Logger.Info("TestWebSocketClient.exe wss://gs-stage-ap-elb-client-test-1560911213.ap-northeast-1.elb.amazonaws.com:443");
            Logger.Info("");
        }

        private static Uri UriTryParse(string path)
        {
            try
            {
                return new Uri(path);
            }
            catch (Exception e)
            {
                Logger.Error(new
                {
                    Event = "uri_try_parse_exception",
                    Exception = e,
                });
                return null;
            }
        }
    }

    internal class Main
    {
        private ClientWebSocket WebSocket { get; set; }
        private Uri Uri { get; set; }

        public Main(Uri uri)
        {
            WebSocket = new ClientWebSocket();
            Uri = uri;
        }

        public async Task MainAsync()
        {
            await Connect();
            //ConnectHttp();
            await Close();
        }

        public void ConnectHttp()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Uri);
                request.Method = WebRequestMethods.Http.Get;
                request.ContentType = "application/json";
                request.Headers[HttpRequestHeader.From] = "WebSocket Test Client";
                request.Timeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
                request.ReadWriteTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    var json = reader.ReadToEnd();

                }

                Logger.Error(new
                {
                    Event = "connect_http_to_server",
                    Uri = Uri,
                });
            }
            catch (Exception e)
            {
                Logger.Error(new
                {
                    Event = "connect_http_exception",
                    Uri = Uri,
                    Exception = e,
                });
            }
        }

        public async Task Connect()
        {
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await WebSocket.ConnectAsync(Uri, cts.Token);

                Logger.Info(new
                {
                    Event = "connect_to_server",
                    Uri = Uri,
                });

                WebSocket.Abort();

                Logger.Info(new
                {
                    Event = "connect_closed_successfully",
                    Uri = Uri,
                });
            }
            catch (Exception e)
            {
                Logger.Error(new
                {
                    Event = "connect_exception",
                    Uri = Uri,
                    Exception = e,
                });
            }
        }

        public async Task Close()
        {
            if (WebSocket.State != WebSocketState.Open)
                return;

            //WebSocket.Abort();
            //await WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
        }
    }
}
