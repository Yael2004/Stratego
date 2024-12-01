using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using StrategoServices.Services.Interfaces;
using StrategoServices.Logic;
using StrategoServices.Data;
using log4net;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ChatService));
        private readonly ChatManager _chatManager;
        private readonly ConcurrentDictionary<int, IChatServiceCallback> _clients = new ConcurrentDictionary<int, IChatServiceCallback>();
        private int _nextGuestId = -1;
        private readonly ConnectedPlayersManager _connectedPlayersManager;

        public ChatService(ConnectedPlayersManager connectedPlayersManager)
        {
            _chatManager = new ChatManager();

            _chatManager.OnClientConnected += HandleClientConnected;
            _chatManager.OnClientDisconnected += HandleClientDisconnected;
            _chatManager.OnMessageBroadcast += BroadcastMessage;
            _connectedPlayersManager = connectedPlayersManager;
        }

        public int Connect(int userId, string username)
        {
            try
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

                    var communicationObject = (ICommunicationObject)callback;
                    communicationObject.Closed += (s, e) => OnClientDisconnected(userId);
                    communicationObject.Faulted += (s, e) => OnClientDisconnected(userId);
                }

                return userId;
            }
            catch (TimeoutException tex)
            {
                _log.Error("Timeout error in Connect method: " + tex.Message);
                return 0;
            }
            catch (CommunicationException cex)
            {
                _log.Error("Communication error in Connect method: " + cex.Message);
                return 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error in Connect method: " + ex.Message);
                return 0;
            }
        }

        public void Disconnect(int userId)
        {
            try
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
            catch (TimeoutException tex)
            {
                _log.Error("Timeout error in Disconnect method: " + tex.Message);
            }
            catch (CommunicationException cex)
            {
                _log.Error("Communication error in Disconnect method: " + cex.Message);
            }
            catch (Exception ex)
            {
                _log.Error("Error in Disconnect method: " + ex.Message);
            }
        }

        public void SendMessage(int userId, string username, string message)
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

                if (!_chatManager.SendMessage(userId, username, message))
                {
                    callback.ChatResponse(new OperationResult(false, "User is not connected."));
                }
            }
            catch (TimeoutException tex)
            {
                _log.Error("Timeout error in SendMessage method: " + tex.Message);
            }
            catch (CommunicationException cex)
            {
                _log.Error("Communication error in SendMessage method: " + cex.Message);
            }
            catch (Exception ex)
            {
                _log.Error("Error in SendMessage method: " + ex.Message);
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
                    _log.Error($"Sending message error: {ex.Message}");
                }
            }
        }

        private void OnClientDisconnected(int userId)
        {
            try
            {
                if (_connectedPlayersManager.RemovePlayer(userId))
                {
                    Console.WriteLine($"Player {userId} removed from connected players list.");
                }
                else
                {
                    Console.WriteLine($"Error: Player {userId} wasn't in the connected players list.");
                }

                IChatServiceCallback callback;
                _clients.TryRemove(userId, out callback);
            }
            catch (TimeoutException tex)
            {
                _log.Error("Timeout error in OnClientDisconnected method: " + tex.Message);
            }
            catch (CommunicationException cex)
            {
                _log.Error("Communication error in OnClientDisconnected method: " + cex.Message);
            }
            catch (Exception ex)
            {
                _log.Error("Error in OnClientDisconnected method: " + ex.Message);
            }
        }
    }
}
