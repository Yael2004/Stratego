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

        // Mocks para los eventos
        private Mock<Action<int, string>> _onClientConnectedMock;
        private Mock<Action<int, string>> _onClientDisconnectedMock;
        private Mock<Action<int, string, string>> _onMessageBroadcastMock;

        [TestInitialize]
        public void SetUp()
        {
            // Crear mocks para los eventos
            _onClientConnectedMock = new Mock<Action<int, string>>();
            _onClientDisconnectedMock = new Mock<Action<int, string>>();
            _onMessageBroadcastMock = new Mock<Action<int, string, string>>();

            // Crear instancia de ChatManager y asignar los mocks a los eventos
            _chatManager = new ChatManager();
            _chatManager.OnClientConnected += _onClientConnectedMock.Object;
            _chatManager.OnClientDisconnected += _onClientDisconnectedMock.Object;
            _chatManager.OnMessageBroadcast += _onMessageBroadcastMock.Object;
        }

        [TestMethod]
        public void Connect_ValidUser_ReturnsTrue_AndTriggersEvent()
        {
            // Arrange
            int userId = 1;
            string username = "TestUser";

            // Act
            var result = _chatManager.Connect(userId, username);

            // Assert
            Assert.IsTrue(result);
            _onClientConnectedMock.Verify(e => e.Invoke(userId, username), Times.Once);
        }

        [TestMethod]
        public void Connect_UserAlreadyConnected_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string username = "TestUser";

            // Conectar el primer usuario
            _chatManager.Connect(userId, username);

            // Act
            var result = _chatManager.Connect(userId, username);

            // Assert
            Assert.IsFalse(result);  // No debería permitir conectar dos veces
            _onClientConnectedMock.Verify(e => e.Invoke(userId, username), Times.Once); // El evento solo debe dispararse una vez
        }

        [TestMethod]
        public void Disconnect_ValidUser_ReturnsTrue_AndTriggersEvent()
        {
            // Arrange
            int userId = 1;
            string username = "TestUser";

            // Conectar al usuario antes de desconectar
            _chatManager.Connect(userId, username);

            // Act
            var result = _chatManager.Disconnect(userId, username);

            // Assert
            Assert.IsTrue(result);
            _onClientDisconnectedMock.Verify(e => e.Invoke(userId, username), Times.Once);
        }

        [TestMethod]
        public void Disconnect_UserNotConnected_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string username = "TestUser";

            // Act
            var result = _chatManager.Disconnect(userId, username);

            // Assert
            Assert.IsFalse(result);  // No se puede desconectar si el usuario no está conectado
            _onClientDisconnectedMock.Verify(e => e.Invoke(It.IsAny<int>(), It.IsAny<string>()), Times.Never);  // No se debe disparar el evento
        }

        [TestMethod]
        public void SendMessage_ConnectedUser_ReturnsTrue_AndBroadcastsMessage()
        {
            // Arrange
            int userId = 1;
            string username = "TestUser";
            string message = "Hello, World!";

            // Conectar al usuario antes de enviar el mensaje
            _chatManager.Connect(userId, username);

            // Act
            var result = _chatManager.SendMessage(userId, username, message);

            // Assert
            Assert.IsTrue(result);
            _onMessageBroadcastMock.Verify(e => e.Invoke(userId, username, message), Times.Once); // El mensaje debería ser transmitido
        }

        [TestMethod]
        public void SendMessage_DisconnectedUser_ReturnsFalse_AndDoesNotBroadcastMessage()
        {
            // Arrange
            int userId = 1;
            string username = "TestUser";
            string message = "Hello, World!";

            // Act
            var result = _chatManager.SendMessage(userId, username, message);

            // Assert
            Assert.IsFalse(result);  // El usuario no está conectado, no puede enviar el mensaje
            _onMessageBroadcastMock.Verify(e => e.Invoke(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never); // No se debe disparar el evento
        }
    }
}
