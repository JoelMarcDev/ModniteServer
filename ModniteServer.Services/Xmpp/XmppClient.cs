using Serilog;
using System.Net.Sockets;
using System.Xml.Linq;

namespace ModniteServer.Xmpp
{
    internal sealed class XmppClient
    {
        public XmppClient(XmppServer owner, Socket socket)
        {
            Server = owner;
            Socket = socket;
        }

        public XmppServer Server { get; }

        public Socket Socket { get; }

        internal void HandleMessage(XElement element, out XElement response)
        {
            bool messageHandled = false;

            switch (element.Name.LocalName)
            {
                case "iq":
                    {
                        string id = element.Attribute("id").Value;
                        string type = element.Attribute("type").Value;

                        if (id == "_xmpp_auth1")
                        {
                            XNamespace authNs = "jabber:iq:auth";
                            var query = element.Element(authNs + "query");

                            string username = query.Element(authNs + "username").Value;
                            string password = query.Element(authNs + "password").Value;
                            string resource = query.Element(authNs + "resource").Value;

                            Log.Information("[XMPP] Login requested for '" + username + "'");

                            var loginSuccessfulResponse = new XElement("iq");
                            loginSuccessfulResponse.Add(new XAttribute("type", "result"), new XAttribute("id", "_xmpp_auth1"));

                            Server.SendXmppMessage(Socket, loginSuccessfulResponse);
                            messageHandled = true;
                        }
                        else
                        {
                            // Handle ping.
                            XNamespace pingNs = "urn:xmpp:ping";
                            var pingElement = element.Element(pingNs + "ping");
                            if (pingElement != null)
                            {
                                string from = element.Attribute("from").Value;
                                string to = element.Attribute("to").Value;

                                var pongResponse = new XElement(
                                    "iq",
                                    new XAttribute("id", id),
                                    new XAttribute("from", from),
                                    new XAttribute("to", to),
                                    new XAttribute("type", "result")
                                );

                                Log.Information("[XMPP] Ping");

                                Server.SendXmppMessage(Socket, pongResponse);
                                messageHandled = true;
                            }

                            // TODO: Handle other <iq/> messages here.
                        }
                    }
                    break;

                case "close":
                    {
                        var closeResponse = new XElement("close", new XAttribute("xmlns", "urn:ietf:params:xml:ns:xmpp-framing"));
                        Server.SendXmppMessage(Socket, closeResponse);
                        messageHandled = true;
                    }
                    break;

                case "presence":
                    {
                        // TODO
                        // <presence>
                        //   <status>json data</status>
                        //   <delay stamp="datetime" xmlns="urn:xmpp:delay" />
                        // </presence>
                    }
                    break;
            }

            if (!messageHandled)
            {
                Log.Warning("[XMPP] Unhandled message received {Message}", element);
            }

            response = null;
        }
    }
}
