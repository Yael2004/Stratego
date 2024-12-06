using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoServices.Logic;
using System;

namespace StrategoServices.Tests
{
    [TestClass]
    public class ChatManagerTests
    {
        private ChatManager _chatManager;

        private Mock<Action<int, string>> _onClientConnectedMock;
        private Mock<Action<int, string>> _onClientDisconnectedMock;
        private Mock<Action<int, string, string>> _onMessageBroadcastMock;

        [TestInitialize]
        public void SetUp()
        {
            _onClientConnectedMock = new Mock<Action<int, string>>();
            _onClientDisconnectedMock = new Mock<Action<int, string>>();
            _onMessageBroadcastMock = new Mock<Action<int, string, string>>();

            _chatManager = new ChatManager();
            _chatManager.OnClientConnected += _onClientConnectedMock.Object;
            _chatManager.OnClientDisconnected += _onClientDisconnectedMock.Object;
            _chatManager.OnMessageBroadcast += _onMessageBroadcastMock.Object;
        }

        [TestMethod]
        public void Test_Connect_ValidUser_ReturnsTrue_AndTriggersEvent()
        {
            int userId = 1;
            string username = "TestUser";

            var result = _chatManager.Connect(userId, username);

            Assert.IsTrue(result);
            _onClientConnectedMock.Verify(e => e.Invoke(userId, username), Times.Once);
        }

        [TestMethod]
        public void Test_Connect_UserAlreadyConnected_ReturnsFalse()
        {
            int userId = 1;
            string username = "TestUser";

            _chatManager.Connect(userId, username);

            var result = _chatManager.Connect(userId, username);

            Assert.IsFalse(result);
            _onClientConnectedMock.Verify(e => e.Invoke(userId, username), Times.Once);
        }

        [TestMethod]
        public void Test_Disconnect_ValidUser_ReturnsTrue_AndTriggersEvent()
        {
            int userId = 1;
            string username = "TestUser";

            _chatManager.Connect(userId, username);

            var result = _chatManager.Disconnect(userId, username);

            Assert.IsTrue(result);
            _onClientDisconnectedMock.Verify(e => e.Invoke(userId, username), Times.Once);
        }

        [TestMethod]
        public void Test_Disconnect_UserNotConnected_ReturnsFalse()
        {
            int userId = 1;
            string username = "TestUser";

            var result = _chatManager.Disconnect(userId, username);

            Assert.IsFalse(result);
            _onClientDisconnectedMock.Verify(e => e.Invoke(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Test_SendMessage_ConnectedUser_ReturnsTrue_AndBroadcastsMessage()
        {
            int userId = 1;
            string username = "TestUser";
            string message = "Hello, World!";

            _chatManager.Connect(userId, username);

            var result = _chatManager.SendMessage(userId, username, message);

            Assert.IsTrue(result);
            _onMessageBroadcastMock.Verify(e => e.Invoke(userId, username, message), Times.Once);
        }

        [TestMethod]
        public void Test_SendMessage_DisconnectedUser_ReturnsFalse_AndDoesNotBroadcastMessage()
        {
            int userId = 1;
            string username = "TestUser";
            string message = "Hello, World!";

            var result = _chatManager.SendMessage(userId, username, message);

            Assert.IsFalse(result);
            _onMessageBroadcastMock.Verify(e => e.Invoke(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
