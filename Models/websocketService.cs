using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSocketSharp.Server;
using WebSocketSharp;

namespace serverapi.Models
{
    public class websocketService
    {
        private static WebSocketSharp.Server.WebSocketServer _Wssv;

        /// <summary>
        /// 啟動websocket server
        /// </summary>
        public static void StartServer()
        {
            _Wssv = new WebSocketServerWrapper("ws://127.0.0.1:7890");
            _Wssv.Start();
        }
        public class NotifyBehavior : WebSocketBehavior
        {
            private static ConcurrentDictionary<string, WebSocketSharp.WebSocket> Users = new ConcurrentDictionary<string, WebSocketSharp.WebSocket>();

            public void RegisterUser(string User)
            {
                if (Users.ContainsKey(User))
                {
                    Users[User].Close();
                }
                Users[User] = this.Context.WebSocket;

                WebSocketSharp.WebSocket SocketSender;
                if (Users.TryGetValue(User, out SocketSender))
                { SocketSender.Send("User Connecting Succeed!"); }
            }

            private void ColseWithErrorMessage(string ErrorMessage)
            {
                this.Context.WebSocket.Send("Connecting Failed, do not correctly pass user info.");
                this.Context.WebSocket.Close();
            }

            protected override void OnOpen()
            {
                try
                {
                    var QueryString = this.Context.QueryString;

                    if (QueryString["User"] != null)
                    {
                        RegisterUser(QueryString["User"]);
                    }
                    else
                    {
                        ColseWithErrorMessage("Connecting Failed, do not correctly pass user info.");
                    }
                }
                catch (Exception ex)
                {
                    ColseWithErrorMessage("Connecting Failed, server error.");
                }
            }

            protected override void OnMessage(MessageEventArgs e)
            {
                WebSocketMessageInfo WebSocketMessage = JsonConvert.DeserializeObject<WebSocketMessageInfo>(e.Data);
                WebSocketSharp.WebSocket SocketReciver;
                if (Users.TryGetValue(WebSocketMessage.Reciver, out SocketReciver))
                {
                    SocketReciver.Send(WebSocketMessage.Content);
                }
                else
                {
                    WebSocketSharp.WebSocket SocketSender;
                    if (Users.TryGetValue(WebSocketMessage.Sender, out SocketSender))
                    {
                        SocketSender.Send("Message Sending Failed.");
                    }
                }
            }

            protected override void OnClose(CloseEventArgs e)
            {
                var User = Users.FirstOrDefault(users => users.Value == this.Context.WebSocket).Key;
                if (User != null)
                {

                    Users.TryRemove(User, out _);
                }
            }

            public static bool IsUserConnected(string User)
            {
                return Users.ContainsKey(User);
            }
        }
        internal class WebSocketServerWrapper : WebSocketSharp.Server.WebSocketServer
        {
            public WebSocketServerWrapper(string url) : base(url)
            {
                AddWebSocketService<NotifyBehavior>("/NotifyBehavior");
            }
        }
    }
}