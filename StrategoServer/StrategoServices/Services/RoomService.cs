using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using StrategoServices.Services.Interfaces;
using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RoomService : IRoomService
    {
        private readonly ConcurrentDictionary<string, Room> _rooms = new ConcurrentDictionary<string, Room>();

        private readonly Lazy<ReportPlayerManager> _reportPlayerManager;

        public RoomService(Lazy<ReportPlayerManager> reportPlayerManager)
        {
            _reportPlayerManager = reportPlayerManager;
        }

        /// <summary>
        /// Creates a room and adds the player to it, sending the room code back to the player.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>bool status</returns>
        public async Task<bool> CreateRoomAsync(int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IRoomServiceCallback>();
            var response = new RoomCreatedResponse();
            bool canCreate = false;

            string roomCode;
            do
            {
                roomCode = GenerateRoomCode();
            } while (_rooms.ContainsKey(roomCode));

            var newRoom = new Room
            {
                Code = roomCode,
                Player1Id = playerId,
                PlayerCallbacks = new List<IRoomServiceCallback> { callback, null }
            };

            try
            {
                if (_rooms.TryAdd(roomCode, newRoom))
                {
                    var communicationObject = (ICommunicationObject)callback;
                    communicationObject.Faulted += (s, e) => HandlePlayerDisconnection(roomCode, playerId);
                    communicationObject.Closed += (s, e) => HandlePlayerDisconnection(roomCode, playerId);

                    response.Result = new OperationResult(true, "Room created successfully");
                    response.RoomCode = roomCode;
                    canCreate = true;
                }
                else
                {
                    response.Result = new OperationResult(false, "Room creation failed");
                }

                await Task.Run(() => callback.RoomCreatedAsync(response));
            }
            catch (TimeoutException ex)
            {
                response.Result = new OperationResult(false, $"{Messages.TimeoutError}  : {ex.Message}");
                await Task.Run(() => callback.RoomCreatedAsync(response));
            }
            catch (CommunicationException ex)
            {
                response.Result = new OperationResult(false, $"{Messages.CommunicationError}  : {ex.Message}");
                await Task.Run(() => callback.RoomCreatedAsync(response));
            }
            catch (Exception ex)
            {
                response.Result = new OperationResult(false, $"{Messages.UnexpectedError}  : {ex.Message}");
                await Task.Run(() => callback.RoomCreatedAsync(response));
            }

            return canCreate;
        }

        /// <summary>
        /// Adds the player to the room and sends a response back to the player.
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="playerId"></param>
        /// <returns>bool status</returns>
        public async Task<bool> JoinRoomAsync(string roomCode, int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IRoomServiceCallback>();
            OperationResult response;
            bool canJoin = false;

            try
            {
                if (_rooms.TryGetValue(roomCode, out var room))
                {
                    if (room.IsFull)
                    {
                        response = new OperationResult(false, "Room is full.");
                    }
                    else if (room.Player1Id == playerId)
                    {
                        response = new OperationResult(false, "Player is already in the room.");
                    }
                    else
                    {
                        room.Player2Id = playerId;
                        room.PlayerCallbacks[1] = callback;
                        room.IsFull = true;

                        var communicationObject = (ICommunicationObject)callback;
                        communicationObject.Faulted += (s, e) => HandlePlayerDisconnection(roomCode, playerId);
                        communicationObject.Closed += (s, e) => HandlePlayerDisconnection(roomCode, playerId);

                        response = new OperationResult(true, "Joined room successfully.");
                        canJoin = true;
                    }
                }
                else
                {
                    response = new OperationResult(false, "Room does not exist.");
                }

                await Task.Run(() => callback.RoomResponseAsync(response));
            }
            catch (TimeoutException ex)
            {
                response = new OperationResult(false, $"{Messages.TimeoutError}  : {ex.Message}");
                await Task.Run(() => callback.RoomResponseAsync(response));
            }
            catch (CommunicationException ex)
            {
                response = new OperationResult(false, $"{Messages.CommunicationError}  : {ex.Message}");
                await Task.Run(() => callback.RoomResponseAsync(response));
            }
            catch (Exception ex)
            {
                response = new OperationResult(false, $"{Messages.UnexpectedError}  : {ex.Message}");
                await Task.Run(() => callback.RoomResponseAsync(response));
            }

            return canJoin;
        }

        /// <summary>
        /// Removes the player from the room and sends a response back to the player.
        /// </summary>
        /// <param name="playerId"></param>
        public void LeaveRoomAsync(int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IRoomServiceCallback>();
            OperationResult response;

            try
            {
                var room = _rooms.Values.FirstOrDefault(r => r.Player1Id == playerId || r.Player2Id == playerId);

                if (room == null)
                {
                    response = new OperationResult(false, "Player is not in any room.");
                }
                else
                {
                    if (room.Player1Id == playerId)
                    {
                        room.Player1Id = 0;
                    }
                    else if (room.Player2Id == playerId)
                    {
                        room.Player2Id = 0;
                    }

                    room.IsFull = false;

                    if (room.Player1Id == 0 && room.Player2Id == 0)
                    {
                        _rooms.TryRemove(room.Code, out _);
                    }

                    response = new OperationResult(true, "Player has left the room.");
                }

                callback.RoomResponseAsync(response);
            }
            catch (TimeoutException ex)
            {
                response = new OperationResult(false, $"{Messages.TimeoutError}  : {ex.Message}");
                callback.RoomResponseAsync(response);
            }
            catch (CommunicationException ex)
            {
                response = new OperationResult(false, $"{Messages.CommunicationError}  : {ex.Message}");
                callback.RoomResponseAsync(response);
            }
            catch (Exception ex)
            {
                response = new OperationResult(false, $"{Messages.UnexpectedError} : {ex.Message}");
                callback.RoomResponseAsync(response);
            }
        }

        /// <summary>
        /// Sends a message to the room.
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="playerId"></param>
        /// <param name="message"></param>
        /// <returns>Task</returns>
        public async Task SendMessageToRoomAsync(string roomCode, int playerId, string message)
        {
            try
            {
                if (_rooms.TryGetValue(roomCode, out var room))
                {
                    foreach (var callback in room.PlayerCallbacks)
                    {
                        if (callback != null)
                        {
                            await Task.Run(() => callback.ReceiveMessageAsync(playerId, message));
                        }
                    }
                }
                else
                {
                    var callback = OperationContext.Current.GetCallbackChannel<IRoomServiceCallback>();
                    await Task.Run(() => callback.RoomResponseAsync(new OperationResult(false, "Room does not exist.")));
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"{Messages.TimeoutError} : {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"{Messages.CommunicationError} : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        private string GenerateRoomCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }

        /// <summary>
        /// Handles player disconnection from the room.
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="playerId"></param>
        private void HandlePlayerDisconnection(string roomCode, int playerId)
        {
            if (_rooms.TryGetValue(roomCode, out var room))
            {
                if (room.Player1Id == playerId)
                    room.Player1Id = 0;
                else if (room.Player2Id == playerId)
                    room.Player2Id = 0;

                if (room.Player1Id == 0 && room.Player2Id == 0)
                {
                    _rooms.TryRemove(roomCode, out _);
                    Console.WriteLine($"Room {roomCode} has been removed due to disconnection.");
                }
                else
                {
                    room.IsFull = false;
                    Console.WriteLine($"Player {playerId} has left room {roomCode} due to disconnection.");
                }
            }
        }

        /// <summary>
        /// Notifies the player already set the new id player connected for getting the new id player connected.
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="connectedPlayerId"></param>
        public void NotifyPlayersOfNewConnectionAsync(string roomCode, int connectedPlayerId)
        {
            try
            {
                if (_rooms.TryGetValue(roomCode, out var room))
                {
                    if (room.PlayerCallbacks[0] != null)
                    {
                        SafeCallbackInvoke(() => room.PlayerCallbacks[0].GetConnectedPlayerId(room.Player2Id));
                    }

                    if (room.PlayerCallbacks[1] != null)
                    {
                        SafeCallbackInvoke(() => room.PlayerCallbacks[1].GetConnectedPlayerId(room.Player1Id));
                    }
                }
                else
                {
                    if (room.PlayerCallbacks[0] != null)
                    {
                        SafeCallbackInvoke(() => room.PlayerCallbacks[0].GetConnectedPlayerId(0));
                    }

                    if (room.PlayerCallbacks[1] != null)
                    {
                        SafeCallbackInvoke(() => room.PlayerCallbacks[1].GetConnectedPlayerId(0));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Messages.CommunicationError} : {ex.Message}");
            }
        }

        /// <summary>
        /// Notifies the players the connection for starting a game
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="gameId"></param>
        public void NotifyOpponentToJoinGameAsync(string roomCode, int gameId)
        {
            try
            {
                if (_rooms.TryGetValue(roomCode, out var room))
                {
                    var result = new OperationResult(true, "Players are joining.");
                    NotifyAllPlayersInRoomAsync(room, (callback) =>
                    {
                        SafeCallbackInvoke(() => callback.NotifyToJoinGame(gameId, result));
                    });
                }
                else
                {
                    var result = new OperationResult(false, "Players joining error.");
                    foreach (var callback in _rooms[roomCode].PlayerCallbacks.Where(c => c != null))
                    {
                        SafeCallbackInvoke(() => callback.NotifyToJoinGame(gameId, result));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to notify all players in a room.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="action"></param>
        private void NotifyAllPlayersInRoomAsync(Room room, Action<IRoomServiceCallback> action)
        {
            foreach (var callback in room.PlayerCallbacks)
            {
                if (callback != null)
                {
                    SafeCallbackInvoke(() => action(callback));
                }
            }
        }

        /// <summary>
        /// Reports a player account.
        /// </summary>
        /// <param name="reporterId"></param>
        /// <param name="reportedId"></param>
        /// <param name="reason"></param>
        public void ReportPlayerAccountAsync(int reporterId, int reportedId, string reason)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IRoomServiceCallback>();
            OperationResult operationResult;

            if (reporterId == reportedId)
            {
                operationResult = new OperationResult(false, "You cannot report yourself.");
                SafeCallbackInvoke(() => callback.NotifyPlayerReported(operationResult));
                return;
            }

            try
            {
                var result = _reportPlayerManager.Value.ReportPlayer(reporterId, reportedId, reason);

                if (!result.IsSuccess)
                {
                    operationResult = new OperationResult(false, result.Error);
                }
                else
                {
                    operationResult = new OperationResult(true, "Player reported successfully.");
                }
            }
            catch (TimeoutException ex)
            {
                operationResult = new OperationResult(false, $"{Messages.TimeoutError}: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                operationResult = new OperationResult(false, $"{Messages.CommunicationError}: {ex.Message}");
            }
            catch (Exception ex)
            {
                operationResult = new OperationResult(false, $"{Messages.UnexpectedError}: {ex.Message}");
            }

            SafeCallbackInvoke(() => callback.NotifyPlayerReported(operationResult));
        }

        /// <summary>
        /// Helper method to safely invoke a callback
        /// </summary>
        /// <param name="callbackAction"></param>
        private void SafeCallbackInvoke(Action callbackAction)
        {
            try
            {
                callbackAction();
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"{Messages.TimeoutError}: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"{Messages.CommunicationError}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Messages.UnexpectedError}: {ex.Message}");
            }
        }

    }
}
