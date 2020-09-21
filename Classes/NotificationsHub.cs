using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRInbox.Classes
{
    [HubName("notificationsHub")]
    public class notificationsHub : Hub
    {
        /// <summary>
        /// https://www.c-sharpcorner.com/article/user-specific-notifications-using-asp-net-mvc-and-signalr/
        /// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#IUserIdProvider
        /// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#inmemory
        /// </summary>
        /// <param name="author"></param>
        /// <param name="message"></param>

        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        
        public void checkIn()
        {
            int Count = _connections.Count;

            var context = GlobalHost.ConnectionManager.GetHubContext<notificationsHub>();
            var clients = context.Clients.All;
        }        
        public void SendMessage(string who, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<notificationsHub>();
            foreach (var connectionId in _connections.GetConnections(who))
            {
                var client = context.Clients.Client(connectionId);
                if (client != null)
                {
                    client.SendMessage(message);
                }
            }
        }

        public override Task OnConnected()
        {
            string name = string.Empty;
            var version = GetClientId();

            if (version != null)
                name = version.ToString();
            else
                name = Context.User.Identity.Name;

            if (_connections.GetConnections(name).Contains(Context.ConnectionId))
                _connections.Remove(name, Context.ConnectionId);

            _connections.Add(name, Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            string name = string.Empty;
            var version = GetClientId();

            if (version != null)
                name = version.ToString();
            else
                name = Context.User.Identity.Name;

            if (_connections.GetConnections(name).Contains(Context.ConnectionId))
                _connections.Remove(name, Context.ConnectionId);

            _connections.Add(name, Context.ConnectionId);
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string name = string.Empty;
            var version = GetClientId();

            if (version != null)
                name = version.ToString();
            else
                name = Context.User.Identity.Name;

            if (_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Remove(name, Context.ConnectionId);
            }

            return base.OnDisconnected(stopCalled);
        }        
        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["empNo"] != null)
            {
                clientId = this.Context.QueryString["empNo"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }
    }    
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();
        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }
        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }
        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }
        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);
                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }    
}