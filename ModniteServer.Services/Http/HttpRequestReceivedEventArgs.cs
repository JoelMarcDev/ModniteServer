using System;
using System.Net;

namespace ModniteServer.Http
{
    public sealed class HttpRequestReceivedEventArgs : EventArgs
    {
        public HttpRequestReceivedEventArgs(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }

        public HttpListenerRequest Request { get; }

        public HttpListenerResponse Response { get; }
    }
}