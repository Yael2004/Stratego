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

        public async Task CreateRoomAsync(string playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IRoomServiceCallback>();
            var response = new RoomCreatedResponse();

            string roomCode;
            do
            {
                roomCode = GenerateRoomCode();
            } while (_rooms.ContainsKey(roomCode));

            var newRoom = new Room
            {
                Code = roomCode,
                Player1Id = playerId
            };

            if (_rooms.TryAdd(roomCode, newRoom))
            {
                var communicationObject = (ICommunicationObject)callback;
                communicationObject.Faulted += (s, e) => HandlePlayerDisconnection(roomCode, playerId);
                communicationObject.Closed += (s, e) => HandlePlayerDisconnection(roomCode, playerId);

                response.Result = new OperationResult(true, "Room created successfully");
                response.RoomCode = roomCode;
            }
            else
            {
                response.Result = new OperationResult(false, "Room creation failed");
            }

            await Task.Run(() => callback.RoomCreatedAsync(response));
        }

        public async Task JoinRoomAsync(string playerId, string roomCode)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IRoomServiceCallback>();
            OperationResult response;

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
                    room.IsFull = true;

                    var communicationObject = (ICommunicationObject)callback;
                    communicationObject.Faulted += (s, e) => HandlePlayerDisconnection(roomCode, playerId);
                    communicationObject.Closed += (s, e) => HandlePlayerDisconnection(roomCode, playerId);

                    response = new OperationResult(true, "Joined room successfully.");
                }
            }
            else
            {
                response = new OperationResult(false, "Room does not exist.");
            }

            await Task.Run(() => callback.RoomResponseAsync(response));
        }

        public void LeaveRoomAsync(string playerId)
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
                    room.Player1Id = null;
                }
                else if (room.Player2Id == playerId)
                {
                    room.Player2Id = null;
                }

                room.IsFull = false;

                if (string.IsNullOrEmpty(room.Player1Id) && string.IsNullOrEmpty(room.Player2Id))
                {
                    _rooms.TryRemove(room.Code, out _);
                }

                response = new OperationResult(true, "Player has left the room.");
            }

            callback.RoomResponseAsync(response);
        }

        public async Task SendMessageToRoomAsync(string playerId, string roomCode, string message)
        {
            var receiverCallback = OperationContext.Current.GetCallbackChannel<IRoomServiceCallback>();

            if (_rooms.TryGetValue(roomCode, out var room))
            {
                string receiverId = room.Player1Id == playerId ? room.Player2Id : room.Player1Id;

                if (receiverId != null)
                {

                    await Task.Run(() => receiverCallback.ReceiveMessageAsync(playerId, message));
                }
            }
            else
            {
                await Task.Run(() => receiverCallback.RoomResponseAsync(new OperationResult(false, "Room does not exist.")));
            }
        }

        private string GenerateRoomCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }

        private void HandlePlayerDisconnection(string roomCode, string playerId)
        {
            if (_rooms.TryGetValue(roomCode, out var room))
            {
                if (room.Player1Id == playerId)
                    room.Player1Id = null;
                else if (room.Player2Id == playerId)
                    room.Player2Id = null;

                if (room.Player1Id == null && room.Player2Id == null)
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
