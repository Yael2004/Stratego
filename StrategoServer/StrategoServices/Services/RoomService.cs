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
            return canCreate;
        }

        public async Task<bool> JoinRoomAsync(string roomCode, int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IRoomServiceCallback>();
            OperationResult response;
            bool canJoin = false;

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
            return canJoin;
        }

        public void LeaveRoomAsync(int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IRoomServiceCallback>();
            OperationResult response;

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

        public async Task SendMessageToRoomAsync(string roomCode, int playerId, string message)
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

    }
}
