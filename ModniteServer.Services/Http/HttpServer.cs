using System;
using System.Net;
using System.Threading.Tasks;

namespace ModniteServer.Http
{
    public sealed class HttpServer : IDisposable
    {
        private bool _isDisposed;
        private readonly HttpListener _listener;

        public HttpServer(string ip, ushort port)
        {
            LocalIP = IPAddress.Parse(ip);
            Port = port;

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://{LocalIP}:{Port}/");
        }

        public event EventHandler<HttpRequestReceivedEventArgs> RequestReceived;

        public IPAddress LocalIP { get; }

        public ushort Port { get; }

        public void Start()
        {
            _listener.Start();
            Task.Run(AcceptConnectionsAsync);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_listener.IsListening)
                {
                    _listener.Stop();
                    _listener.Close();
                }

                _isDisposed = true;
            }
        }

        private async Task AcceptConnectionsAsync()
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                var task = Task.Run(() =>
                {
                    RequestReceived?.Invoke(this, new HttpRequestReceivedEventArgs(context.Request, context.Response));
                });
            }
        }
    }
}