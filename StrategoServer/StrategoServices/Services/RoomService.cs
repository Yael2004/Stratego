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
                response.Result = new OperationResult(false, $"Timeout error while creating room: {ex.Message}");
                await Task.Run(() => callback.RoomCreatedAsync(response));
            }
            catch (CommunicationException ex)
            {
                response.Result = new OperationResult(false, $"Communication error while creating room: {ex.Message}");
                await Task.Run(() => callback.RoomCreatedAsync(response));
            }
            catch (Exception ex)
            {
                response.Result = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
                await Task.Run(() => callback.RoomCreatedAsync(response));
            }

            return canCreate;
        }

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
                response = new OperationResult(false, $"Timeout error while joining room: {ex.Message}");
                await Task.Run(() => callback.RoomResponseAsync(response));
            }
            catch (CommunicationException ex)
            {
                response = new OperationResult(false, $"Communication error while joining room: {ex.Message}");
                await Task.Run(() => callback.RoomResponseAsync(response));
            }
            catch (Exception ex)
            {
                response = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
                await Task.Run(() => callback.RoomResponseAsync(response));
            }

            return canJoin;
        }

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
                response = new OperationResult(false, $"Timeout error while leaving room: {ex.Message}");
                callback.RoomResponseAsync(response);
            }
            catch (CommunicationException ex)
            {
                response = new OperationResult(false, $"Communication error while leaving room: {ex.Message}");
                callback.RoomResponseAsync(response);
            }
            catch (Exception ex)
            {
                response = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
                callback.RoomResponseAsync(response);
            }
        }

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
                Console.WriteLine($"Timeout error while sending message to room: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Communication error while sending message to room: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error while sending message to room: {ex.Message}");
            }
        }

        private string GenerateRoomCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }

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
                Console.WriteLine($"Unexpected error in NotifyPlayersOfNewConnectionAsync: {ex.Message}");
            }
        }

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
                Console.WriteLine($"Unexpected error in NotifyOpponentToJoinGameAsync: {ex.Message}");
            }
        }

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
                operationResult = new OperationResult(false, $"Timeout error while reporting player: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                operationResult = new OperationResult(false, $"Communication error while reporting player: {ex.Message}");
            }
            catch (Exception ex)
            {
                operationResult = new OperationResult(false, $"An unexpected error occurred while reporting the player: {ex.Message}");
            }

            SafeCallbackInvoke(() => callback.NotifyPlayerReported(operationResult));
        }

        private void SafeCallbackInvoke(Action callbackAction)
        {
            try
            {
                callbackAction();
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout error during callback: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Communication error during callback: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during callback: {ex.Message}");
            }
        }

    }
}
