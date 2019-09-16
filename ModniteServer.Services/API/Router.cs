using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace ModniteServer.API
{
    internal delegate void RouteCallback(HttpListenerRequest request, HttpListenerResponse response);

    internal sealed class Route
    {
        public Dictionary<string, Route> Routes { get; set; } = new Dictionary<string, Route>();

        public IList<RouteCallback> Callback { get; set; } = new List<RouteCallback>();

        internal RouteAttribute Attribute { get; set; }
    }

    internal sealed class Router
    {
        private readonly Route _root;

        private Router()
        {
            _root = new Route();
        }

        public static Router Build()
        {
            var router = new Router();

            //var apiFileInfo = new FileInfo("ModniteServer.API.dll");
            //Assembly.LoadFile(apiFileInfo.FullName);

            var controllerMethods = from a in AppDomain.CurrentDomain.GetAssemblies()
                                    from t in a.GetTypes()
                                    from m in t.GetMethods()
                                    where m.GetCustomAttributes(typeof(RouteAttribute), false).Length > 0
                                    select m;

            foreach (var methodInfo in controllerMethods)
            {
                var controller = (Controller)Activator.CreateInstance(methodInfo.DeclaringType);
                var routes = methodInfo.GetCustomAttributes<RouteAttribute>();
                foreach (var route in routes)
                {
                    router.AddRoute(route.Route, (request, response) =>
                    {
                        if (string.Equals(request.HttpMethod, route.Method))
                        {
                            controller.HandleRequest(request, response, methodInfo);
                        }
                    }, route);
                }
            }

            return router;
        }

        public bool TryInvokeRoute(string path, HttpListenerRequest request, HttpListenerResponse response, out RouteAttribute attribute)
        {
            var route = Find(_root, new Queue<string>(path.Split('/')));
            if (route == null)
            {
                attribute = null;
                return false;
            }

            foreach (var callback in route.Callback)
            {
                callback.Invoke(request, response);
            }

            attribute = route.Attribute;
            return true;
        }

        private Route Find(Route route, Queue<string> path)
        {
            if (path.Count == 0)
                return route;

            string name = path.Dequeue();

            if (route.Routes.ContainsKey(name))
            {
                return Find(route.Routes[name], path);
            }
            else if (route.Routes.ContainsKey("*"))
            {
                return Find(route.Routes["*"], path);
            }

            return null;
        }

        private void AddRoute(string path, RouteCallback callback, RouteAttribute attribute)
        {
            AddRoute(_root, new Queue<string>(path.Split('/')), callback, attribute);
        }

        private void AddRoute(Route route, Queue<string> path, RouteCallback callback, RouteAttribute attribute)
        {
            if (path.Count == 0)
            {
                route.Attribute = attribute;
                route.Callback.Add(callback);
                return;
            }

            string name = path.Dequeue();

            if (!route.Routes.ContainsKey(name))
                route.Routes.Add(name, new Route());

            AddRoute(route.Routes[name], path, callback, attribute);
        }
    }
}