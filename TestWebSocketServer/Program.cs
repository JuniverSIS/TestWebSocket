using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebSocketServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var main = new Main();
            main.MainAsync();

            Console.ReadLine();
        }
    }

    internal class Main
    {
        private HttpListener Listener { get; set; }
        private int _port = 16449;

        public async Task MainAsync()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add("http://+:" + _port + "/");
            Listener.TimeoutManager.IdleConnection = TimeSpan.FromSeconds(30);
            Listener.Start();

            Logger.Info(new
            {
                Event = "WebSocket server started.",
                Port = _port,
            });

            Start();
        }

        private async Task Start()
        {
            while (true)
            {
                try
                {
                    var context = await Listener.GetContextAsync();

                    if (ContinueIfHealthChecker(context))
                        continue;

                    Logger.Debug(new
                    {
                        Event = "client_connected",
                        LocalEndPoint = context.Request.LocalEndPoint,
                        RemoteEndPoint = context.Request.RemoteEndPoint,
                        UserAgent = context.Request.UserAgent,
                        Headers = context.Request.Headers,
                    });

                    if (ContinueIfInvalidRequest(context))
                        continue;

                    // WebSocket 접속만 확인한 후 바로 끊음
                    var socketContext = await context.AcceptWebSocketAsync(null);
                    //socketContext.WebSocket.Abort();
                    //context.Response.StatusCode = (int)HttpStatusCode.OK;
                    //context.Response.Close();

                    //await socketContext.WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);

                    Logger.Info(new
                    {
                        Event = "websocket_client_connected_test_ok",
                        RemoteEndPoint = context.Request.RemoteEndPoint,
                    });
                }
                catch (Exception e)
                {
                    Logger.Error(new
                    {
                        Event = "websocket_exception",
                        Exception = e,
                    });
                }
            }
        }

        private static long _healthCheckCount = 0;

        private static bool ContinueIfHealthChecker(HttpListenerContext context)
        {
            var isHealthChecker = context.Request.UserAgent == "ELB-HealthChecker/2.0";
            if (isHealthChecker == false)
                return false;

            Interlocked.Increment(ref _healthCheckCount);

            Logger.Debug(new
            {
                Event = "health_checker_connected",
                LocalEndPoint = context.Request.LocalEndPoint,
                RemoteEndPoint = context.Request.RemoteEndPoint,
                UserAgent = context.Request.UserAgent,
                Count = _healthCheckCount,
            });

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Close();
            return true;
        }

        private static bool ContinueIfInvalidRequest(HttpListenerContext context)
        {
            if (context.Request.IsWebSocketRequest)
                return false;

            Logger.Warn(new
            {
                Event = "client_connected_invalid_request",
                RemoteEndPoint = context.Request.RemoteEndPoint,
            });

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Close();
            return true;
        }
    }
}
