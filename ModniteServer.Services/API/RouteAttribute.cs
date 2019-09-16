using System;

namespace ModniteServer.API
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RouteAttribute : Attribute
    {
        public RouteAttribute(string method, string route)
        {
            Method = method;
            Route = route;
        }

        internal RouteAttribute(string method, string route, bool isHidden)
            : this(method, route)
        {
            IsHidden = isHidden;
        }

        public string Method { get; }

        public string Route { get; }

        internal bool IsHidden { get; }
    }
}