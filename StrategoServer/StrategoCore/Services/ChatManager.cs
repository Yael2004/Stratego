using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoCore.Services
{
    public class ChatManager
    {
        private readonly HashSet<int> _connectedClients = new HashSet<int>();
        public event Action<int, string> OnClientConnected;
        public event Action<int, string> OnClientDisconnected;
        public event Action<int, string, string> OnMessageBroadcast;

        public bool Connect(int userId, string username)
        {
            if (!_connectedClients.Contains(userId))
            {
                _connectedClients.Add(userId);
                OnClientConnected?.Invoke(userId, username);
                return true;
            }

            return false;
        }

        public bool Disconnect(int userId, string username)
        {
            if (_connectedClients.Contains(userId))
            {
                _connectedClients.Remove(userId);
                OnClientDisconnected?.Invoke(userId, username);
                return true;
            }

            return false;
        }

        public bool SendMessage(int userId, string username, string message)
        {
            if (_connectedClients.Contains(userId))
            {
                BroadcastMessage(userId, username, message);
                return true;
            }

            return false;
        }

        private void BroadcastMessage(int senderId, string username, string message)
        {
            OnMessageBroadcast?.Invoke(senderId, username, message);
        }
    }
}