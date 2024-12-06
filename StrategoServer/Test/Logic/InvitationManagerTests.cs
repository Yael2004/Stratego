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
            _playerRepositoryMock = new Mock<PlayerRepository>();

            _invitationManager = new InvitationManager(new Lazy<PlayerRepository>(() => _playerRepositoryMock.Object));
        }

        [TestMethod]
        public void Test_GetPlayerMail_PlayerExists_ReturnsMail()
        {
            int playerId = 1;
            string expectedMail = "player1@example.com";
            var expectedResult = Result<string>.Success(expectedMail);

            _playerRepositoryMock.Setup(repo => repo.GetMailByPlayerId(playerId))
                .Returns(expectedResult);

            var result = _invitationManager.GetPlayerMail(playerId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedMail, result.Value);
        }

        [TestMethod]
        public void Test_GetPlayerMail_PlayerNotFound_ReturnsFailure()
        {
            int playerId = 1;
            var expectedResult = Result<string>.Failure("Player not found");

            _playerRepositoryMock.Setup(repo => repo.GetMailByPlayerId(playerId))
                .Returns(expectedResult);

            var result = _invitationManager.GetPlayerMail(playerId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void Test_GetPlayerMail_InvalidPlayerId_ReturnsFailure()
        {
            int playerId = -1;
            var expectedResult = Result<string>.Failure("Invalid player ID");

            _playerRepositoryMock.Setup(repo => repo.GetMailByPlayerId(playerId))
                .Returns(expectedResult);

            var result = _invitationManager.GetPlayerMail(playerId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid player ID", result.Error);
        }
    }
}
