﻿using System;
using System.Net.Sockets;

namespace ModniteServer.Websockets
{
    public sealed class WebsocketMessageNewConnectionEventArgs : EventArgs
    {
        public WebsocketMessageNewConnectionEventArgs(Socket socket, string authorization)
        {
            Socket = socket;
            Authorization = authorization;
        }

        public Socket Socket { get; }

        public string Authorization { get; }
    }
}