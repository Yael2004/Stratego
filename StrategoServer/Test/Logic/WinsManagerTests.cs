using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using Utilities;
using StrategoServices.Logic;
using System;

namespace StrategoServices.Tests
{
    [TestClass]
    public class WinsManagerTests
    {
        private Mock<GamesRepository> _gamesRepositoryMock;
        private WinsManager _winsManager;

        [TestInitialize]
        public void Setup()
        {
            _gamesRepositoryMock = new Mock<GamesRepository>();
            _winsManager = new WinsManager(new Lazy<GamesRepository>(() => _gamesRepositoryMock.Object));
        }

        [TestMethod]
        public void Test_IncrementWins_ValidAccountId_ReturnsSuccess()
        {
            int accountId = 1;
            var result = Result<string>.Success("Win incremented successfully");

            _gamesRepositoryMock.Setup(repo => repo.IncrementWonGames(accountId))
                .Returns(result);

            var actualResult = _winsManager.IncrementWins(accountId);

            Assert.IsTrue(actualResult.IsSuccess);
            Assert.AreEqual("Win incremented successfully", actualResult.Value);
        }

        [TestMethod]
        public void Test_IncrementWins_FailedIncrement_ReturnsFailure()
        {
            int accountId = 1;
            var result = Result<string>.Failure("Failed to increment wins");

            _gamesRepositoryMock.Setup(repo => repo.IncrementWonGames(accountId))
                .Returns(result);

            var actualResult = _winsManager.IncrementWins(accountId);

            Assert.IsFalse(actualResult.IsSuccess);
            Assert.AreEqual("Failed to increment wins", actualResult.Error);
        }

        [TestMethod]
        public void Test_IncrementDefeats_ValidAccountId_ReturnsSuccess()
        {
            int accountId = 1;
            var result = Result<string>.Success("Defeat incremented successfully");

            _gamesRepositoryMock.Setup(repo => repo.IncrementDeafeatGames(accountId))
                .Returns(result);

            var actualResult = _winsManager.IncrementDefeats(accountId);

            Assert.IsTrue(actualResult.IsSuccess);
            Assert.AreEqual("Defeat incremented successfully", actualResult.Value);
        }

        [TestMethod]
        public void Test_IncrementDefeats_FailedIncrement_ReturnsFailure()
        {
            int accountId = 1;
            var result = Result<string>.Failure("Failed to increment defeats");

            _gamesRepositoryMock.Setup(repo => repo.IncrementDeafeatGames(accountId))
                .Returns(result);

            var actualResult = _winsManager.IncrementDefeats(accountId);

            Assert.IsFalse(actualResult.IsSuccess);
            Assert.AreEqual("Failed to increment defeats", actualResult.Error);
        }
    }
}
