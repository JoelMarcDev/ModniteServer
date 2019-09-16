using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace ModniteServer.API
{
    /// <summary>
    /// Provides a base implementation of an API controller.
    /// </summary>
    public abstract class Controller
    {
        internal void HandleRequest(HttpListenerRequest request, HttpListenerResponse response, MethodInfo methodInfo)
        {
            Request = request;
            Response = response;

            Query = new Dictionary<string, string>();
            foreach (string key in request.QueryString)
            {
                Query.Add(key, request.QueryString[key]);
            }

            if (Request.ContentType == "application/x-www-form-urlencoded")
            {
                byte[] buffer = new byte[Request.ContentLength64];
                Request.InputStream.Read(buffer, 0, buffer.Length);
                string[] formData = Encoding.UTF8.GetString(buffer).Split('&');
                foreach (string field in formData)
                {
                    if (string.IsNullOrWhiteSpace(field))
                        continue;

                    string name = field.Substring(0, field.IndexOf('='));
                    string value = field.Substring(field.IndexOf('=') + 1);
                    Query.Add(name, value);
                }
            }

            methodInfo.Invoke(this, null);
        }

        protected bool Authorize()
        {
            return true;

            //string authorization = Request.Headers["Authorization"];

            //if (authorization != null && authorization.StartsWith("bearer "))
            //{
            //    string token = authorization.Split(' ')[1];

            //    if (OAuthManager.IsTokenValid(token))
            //        return true;
            //}

            //Response.StatusCode = 403;
            //return false;
        }

        protected HttpListenerRequest Request { get; private set; }

        protected HttpListenerResponse Response { get; private set; }

        protected IDictionary<string, string> Query { get; private set; }
    }
}