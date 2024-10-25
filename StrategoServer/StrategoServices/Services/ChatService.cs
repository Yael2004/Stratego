using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using StrategoServices.Services.Interfaces;
using StrategoServices.Logic;
using StrategoServices.Data;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        private readonly ChatManager _chatManager;
        private readonly ConcurrentDictionary<int, IChatServiceCallback> _clients = new ConcurrentDictionary<int, IChatServiceCallback>();
        private int _nextGuestId = -1;

        public ChatService()
        {
            _chatManager = new ChatManager();

            _chatManager.OnClientConnected += HandleClientConnected;
            _chatManager.OnClientDisconnected += HandleClientDisconnected;
            _chatManager.OnMessageBroadcast += BroadcastMessage;
        }

        public int Connect(int userId, string username)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

            if (userId == 0)
            {
                lock (this)
                {
                    userId = _nextGuestId--;
                }
            }

            if (_clients.ContainsKey(userId))
            {
                callback.ChatResponse(new OperationResult(false, "User is already connected."));
            }
            else
            {
                _clients[userId] = callback;

                if (!_chatManager.Connect(userId, username))
                {
                    callback.ChatResponse(new OperationResult(false, "Failed to connect user."));
                }
            }

            return userId;
        }



        public void Disconnect(int userId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

            if (_clients.TryRemove(userId, out _))
            {
                _chatManager.Disconnect(userId, "");
            }
            else
            {
                callback.ChatResponse(new OperationResult(false, "User is not connected."));
            }
        }

        public void SendMessage(int userId, string username, string message)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

            if (!_chatManager.SendMessage(userId, username, message))
            {
                callback.ChatResponse(new OperationResult(false, "User is not connected."));
            }
        }

        private void HandleClientConnected(int userId, string username)
        {
            Console.WriteLine($"Client {username} (ID: {userId}) has connected to the chat.");
        }

        private void HandleClientDisconnected(int userId, string username)
        {
            Console.WriteLine($"Client {username} (ID: {userId}) has disconnected from the chat.");
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
