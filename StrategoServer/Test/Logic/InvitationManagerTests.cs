using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoServices.Logic;
using StrategoDataAccess;
using Utilities;
using System;

namespace StrategoServices.Tests
{
    [TestClass]
    public class InvitationManagerTests
    {
        private InvitationManager _invitationManager;
        private Mock<PlayerRepository> _playerRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            // Crear el Mock para el repositorio de jugadores
            _playerRepositoryMock = new Mock<PlayerRepository>();

            // Inicializar el InvitationManager con el Mock
            _invitationManager = new InvitationManager(new Lazy<PlayerRepository>(() => _playerRepositoryMock.Object));
        }

        [TestMethod]
        public void GetPlayerMail_PlayerExists_ReturnsMail()
        {
            // Arrange
            int playerId = 1;
            string expectedMail = "player1@example.com";
            var expectedResult = Result<string>.Success(expectedMail);

            // Configurar el comportamiento del Mock para devolver el correo del jugador
            _playerRepositoryMock.Setup(repo => repo.GetMailByPlayerId(playerId))
                .Returns(expectedResult);

            // Act
            var result = _invitationManager.GetPlayerMail(playerId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedMail, result.Value);
        }

        [TestMethod]
        public void GetPlayerMail_PlayerNotFound_ReturnsFailure()
        {
            // Arrange
            int playerId = 1;
            var expectedResult = Result<string>.Failure("Player not found");

            // Configurar el comportamiento del Mock para devolver un error (jugador no encontrado)
            _playerRepositoryMock.Setup(repo => repo.GetMailByPlayerId(playerId))
                .Returns(expectedResult);

            // Act
            var result = _invitationManager.GetPlayerMail(playerId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void GetPlayerMail_InvalidPlayerId_ReturnsFailure()
        {
            // Arrange
            int playerId = -1;  // ID no válido
            var expectedResult = Result<string>.Failure("Invalid player ID");

            // Configurar el comportamiento del Mock para devolver un error por ID inválido
            _playerRepositoryMock.Setup(repo => repo.GetMailByPlayerId(playerId))
                .Returns(expectedResult);

            // Act
            var result = _invitationManager.GetPlayerMail(playerId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid player ID", result.Error);
        }
    }
}
