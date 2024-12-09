using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.RoomService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest.Server
{
    [TestClass]
    public class RoomServiceClientTests : IRoomServiceCallback
    {
        private RoomServiceClient _client;

        private int _connectedPlayerId;
        private OperationResult _playerReportedResult;
        private int _gameId;
        private int _playerId;
        private RoomCreatedResponse _roomCreatedResponse;
        private OperationResult _roomResponse;

        [TestInitialize]
        public void Setup()
        {
            _client = new RoomServiceClient(new InstanceContext(this));
        }

        [TestMethod]
        public async Task Test_CreateRoom_Succes()
        {
            var result = await _client.CreateRoomAsync(1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Test_JoinRoom_Success()
        {
            var result = await _client.CreateRoomAsync(1);
            await Task.Delay(4000);
            var resultBool = await _client.JoinRoomAsync(_roomCreatedResponse.RoomCode, 2);
            Assert.IsTrue(resultBool);
        }

        [TestMethod]
        public async Task Test_LeaveRoom_Success()
        {
            await _client.CreateRoomAsync(1);
            await _client.JoinRoomAsync(_roomCreatedResponse.RoomCode, 2);
            _client.LeaveRoomAsync(2);
            await Task.Delay(4000);
            Assert.IsTrue(_roomResponse.IsSuccess);
        }

        [TestMethod]
        public async Task Test_SendMessageRoom_Success()
        {
            await _client.CreateRoomAsync(1);
            await Task.Delay(6000);
            await _client.SendMessageToRoomAsync(_roomCreatedResponse.RoomCode, 1, "Hello");
            Assert.AreEqual(1, _playerId);
        }

        [TestMethod]
        public async Task Test_NotifyNewConnection()
        {
            await _client.CreateRoomAsync(1);
            await Task.Delay(2000);
            await _client.JoinRoomAsync(_roomCreatedResponse.RoomCode, 2);
            await Task.Delay(2000);
            _client.NotifyPlayersOfNewConnectionAsync(_roomCreatedResponse.RoomCode,1);
            await Task.Delay(2000);
            Assert.AreEqual(1, _connectedPlayerId);
        }

        public void GetConnectedPlayerId(int connectedPlayerId)
        {
            _connectedPlayerId = connectedPlayerId;
        }

        public void NotifyPlayerReported(OperationResult result)
        {
            _playerReportedResult = result;
        }

        public void NotifyToJoinGame(int gameId, OperationResult result)
        {
            _gameId = gameId;
        }

        public void ReceiveMessageAsync(int playerId, string message)
        {
            _playerId = playerId;
        }

        public void RoomCreatedAsync(RoomCreatedResponse response)
        {
            _roomCreatedResponse = response;
        }

        public void RoomResponseAsync(OperationResult response)
        {
            _roomResponse = response;
        }
    }
}
