using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using StrategoServices.Logic;
using Utilities;
using System;

namespace Tests.Logic
{
    [TestClass]
    public class InvitationManagerTests
    {
        private Mock<PlayerRepository> _mockPlayerRepository;
        private InvitationManager _invitationManager;

        [TestInitialize]
        public void Setup()
        {
            _mockPlayerRepository = new Mock<PlayerRepository>(new Lazy<StrategoEntities>());
            _invitationManager = new InvitationManager(new Lazy<PlayerRepository>(() => _mockPlayerRepository.Object));
        }

        [TestMethod]
        public void Test_GetPlayerMail_ShouldReturnSuccess_WhenPlayerExists()
        {
            _mockPlayerRepository.Setup(repo => repo.GetMailByPlayerId(1))
                .Returns(Result<string>.Success("player@example.com"));

            var result = _invitationManager.GetPlayerMail(1);

            Assert.AreEqual("player@example.com", result.Value);
        }

        [TestMethod]
        public void Test_GetPlayerMail_ShouldReturnFailure_WhenPlayerDoesNotExist()
        {
            _mockPlayerRepository.Setup(repo => repo.GetMailByPlayerId(1))
                .Returns(Result<string>.Failure("Player not found"));

            var result = _invitationManager.GetPlayerMail(1);

            Assert.AreEqual("Player not found", result.Error);
        }
    }
}
