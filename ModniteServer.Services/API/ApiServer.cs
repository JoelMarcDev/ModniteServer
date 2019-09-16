using ModniteServer.Http;
using Serilog;
using System;
using System.Text;

namespace ModniteServer.API
{
    public sealed class ApiServer : IDisposable
    {
        private bool _isDisposed;
        private bool _serverStarted;

        private readonly HttpServer _server;
        private readonly Router _router;

        public ApiServer(ushort port)
        {
            Port = port;
            _router = Router.Build();

            _server = new HttpServer("127.0.0.1", Port);
            _server.RequestReceived += OnRequestReceived;
        }

        public ushort Port { get; }

        public void Start()
        {
            if (_serverStarted)
                throw new InvalidOperationException("Server already started");

            _server.Start();
            _serverStarted = true;
        }

        public ILogger Logger
        {
            get { return Log.Logger; }
            set { Log.Logger = value; }
        }

        public bool LogHttpRequests { get; set; }

        public bool Log404 { get; set; }

        public string AlternativeRoute { get; set; } = "";

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _server.Dispose();
                _isDisposed = true;
            }
        }

        private void OnRequestReceived(object sender, HttpRequestReceivedEventArgs e)
        {
            string path = e.Request.Url.AbsolutePath;

            bool routeFound = true;
            if (!_router.TryInvokeRoute(path, e.Request, e.Response, out RouteAttribute attribute))
            {
                // Try the alternative route.
                string alternativePath = path.Replace(AlternativeRoute, "");
                routeFound = _router.TryInvokeRoute(alternativePath, e.Request, e.Response, out attribute);
            }

            if (!routeFound)
            {
                if (Log404)
                {
                    string body = "";
                    if (e.Request.ContentLength64 > 0)
                    {
                        byte[] buffer = new byte[e.Request.ContentLength64];
                        e.Request.InputStream.Read(buffer, 0, buffer.Length);
                        body = Encoding.UTF8.GetString(buffer);
                    }

                    Log.Warning($"404 for {e.Request.HttpMethod} {e.Request.Url.AbsolutePath} {{Method}}{{Url}}{{Query}}{{Body}}", e.Request.HttpMethod, e.Request.Url.AbsolutePath, e.Request.Url.Query, body);
                }

                e.Response.StatusCode = 404;
            }
            else
            {
                if (LogHttpRequests && (attribute == null || !attribute.IsHidden))
                {
                    Log.Information($"{e.Request.HttpMethod} {e.Request.Url.AbsolutePath} {{Method}}{{Url}}{{Query}}", e.Request.HttpMethod, e.Request.Url.AbsolutePath, e.Request.Url.Query);
                }
            }

            e.Response.Close();
        }
    }
}