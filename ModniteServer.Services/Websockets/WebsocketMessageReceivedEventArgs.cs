using System;
using System.Net.Sockets;

namespace ModniteServer.Websockets
{
    public sealed class WebsocketMessageReceivedEventArgs : EventArgs
    {
        public WebsocketMessageReceivedEventArgs(Socket socket, WebsocketMessage message)
        {
            Socket = socket;
            Message = message;
        }

        public Socket Socket { get; }

        public WebsocketMessage Message { get; }
    }
}