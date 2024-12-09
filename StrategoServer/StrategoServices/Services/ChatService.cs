using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using StrategoServices.Services.Interfaces;
using StrategoServices.Logic;
using StrategoServices.Data;
using log4net;
using Utilities;

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
        private readonly object _lock = new object();

        public ChatService(ConnectedPlayersManager connectedPlayersManager)
        {
            _chatManager = new ChatManager();

            _chatManager.OnClientConnected += HandleClientConnected;
            _chatManager.OnClientDisconnected += HandleClientDisconnected;
            _chatManager.OnMessageBroadcast += BroadcastMessage;
            _connectedPlayersManager = connectedPlayersManager;
        }

        /// <summary>
        /// Makes a connection with the user and chat
        /// </summary>
        /// <param name="userId">Connected user id, if id is equal to 0, then assign an id as a guest.</param>
        /// <param name="username">Connected user name</param>
        /// <returns>Assigned user id, if there was an error, it returns 0</returns>
        public int Connect(int userId, string username)
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

                if (userId == 0)
                {
                    lock (_lock)
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

                    var playerAdded = _connectedPlayersManager.AddPlayer(userId, username);

                    if (!playerAdded)
                    {
                        callback.ChatResponse(new OperationResult(false, "Failed to add player to connected players list."));
                    }

                    var communicationObject = (ICommunicationObject)callback;
                    communicationObject.Closed += (s, e) => OnClientDisconnected(userId);
                    communicationObject.Faulted += (s, e) => OnClientDisconnected(userId);
                }

                return userId;
            }
            catch (TimeoutException tex)
            {
                _log.Error(Messages.TimeoutError, tex);
                return 0;
            }
            catch (CommunicationException cex)
            {
                _log.Error(Messages.CommunicationError, cex);
                return 0;
            }
            catch (Exception ex)
            {
                _log.Fatal(Messages.UnexpectedError, ex);
                return 0;
            }
        }

        /// <summary>
        /// Disconnect an user from the chat, notices client if user is not connected.
        /// </summary>
        /// <param name="userId">Connected user id.</param>
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
                _log.Error(Messages.TimeoutError, tex);
            }
            catch (CommunicationException cex)
            {
                _log.Error(Messages.CommunicationError, cex);
            }
            catch (Exception ex)
            {
                _log.Fatal(Messages.UnexpectedError, ex);
            }
        }

        /// <summary>
        /// Sends a message to the chat, notices client if user is not connected.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="userId">Connected user id.</param>
        /// <param name="username">Connected user name.</param>
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
                _log.Error(Messages.CommunicationError, tex);
            }
            catch (CommunicationException cex)
            {
                _log.Error(Messages.CommunicationError, cex);
            }
            catch (Exception ex)
            {
                _log.Fatal(Messages.UnexpectedError, ex);
            }
        }

        /// <summary>
        /// Shows connected player advice
        /// </summary>
        /// <param name="userId">Connected user id</param>
        /// <param name="username">Connected user name</param>
        private static void HandleClientConnected(int userId, string username)
        {
            Console.WriteLine($"Client {username} (ID: {userId}) has connected to the chat.");
        }

        /// <summary>
        /// Shows disconnected player advice
        /// <param name="userId"/>Connected user id</param>
        /// <param name="username"/>Connected user name</param>
        private static void HandleClientDisconnected(int userId, string username)
        {
            Console.WriteLine($"Client {username} (ID: {userId}) has disconnected from the chat.");
        }

        /// <summary>
        /// Sends a message to all connected clients
        /// <param name="senderId"/>Connected user id</param>
        /// <param name="username"/>Connected user name</param>
        /// <param name="message"/>Message to send</param>
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
                    _log.Fatal($"Sending message error: ", ex);
                }
            }
        }

        /// <summary>
        /// Handles client disconnection and removes it from the connected players list
        /// <param name="userId"/>Connected user id</param>
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
                _log.Error(Messages.TimeoutError, tex);
            }
            catch (CommunicationException cex)
            {
                _log.Error(Messages.CommunicationError, cex);
            }
            catch (Exception ex)
            {
                _log.Fatal(Messages.UnexpectedError, ex);
            }
        }
    }
}
