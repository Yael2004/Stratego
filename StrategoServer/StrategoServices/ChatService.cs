using StrategoServices.Interfaces.StrategoServices;
using StrategoServices.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using StrategoCore.Services;

namespace StrategoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        private readonly ChatManager _chatManager;
        private readonly Dictionary<int, IChatServiceCallback> _clients = new Dictionary<int, IChatServiceCallback>();

        public ChatService()
        {
            _chatManager = new ChatManager();

            _chatManager.OnClientConnected += HandleClientConnected;
            _chatManager.OnClientDisconnected += HandleClientDisconnected;
            _chatManager.OnMessageBroadcast += BroadcastMessage;
        }

        public void Connect(int userId, string username)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

            _clients[userId] = callback;

            if (!_chatManager.Connect(userId, username))
            {
                throw new InvalidOperationException("User is already connected.");
            }
        }

        public void Disconnect(int userId)
        {
            if (_clients.ContainsKey(userId))
            {
                _clients.Remove(userId);  
                _chatManager.Disconnect(userId, "");  
            }
            else
            {
                throw new InvalidOperationException("User is not connected.");
            }
        }

        public void SendMessage(int userId, string username, string message)
        {
            if (!_chatManager.SendMessage(userId, username, message))
            {
                throw new InvalidOperationException("User is not connected.");
            }
        }

        private void HandleClientConnected(int userId, string username)
        {
            Console.WriteLine($"Client {username} (ID: {userId}) has connected to the chat.");
        }

        private void HandleClientDisconnected(int userId, string username)
        {
            Console.WriteLine($"Cliente {username} (ID: {userId}) has disconnected from the chat.");
        }

        private void BroadcastMessage(int senderId, string username, string message)
        {
            foreach (var client in _clients.Values)
            {
                try
                {
                    client.ReceiveMessage($"{username}: ", message); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Sending message error: {ex.Message}");
                }
            }
        }

    }
}