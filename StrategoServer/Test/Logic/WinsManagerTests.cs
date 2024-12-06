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
        public void IncrementWins_ValidAccountId_ReturnsSuccess()
        {
            // Arrange
            int accountId = 1;
            var result = Result<string>.Success("Win incremented successfully");

            // Setup mock to return success result
            _gamesRepositoryMock.Setup(repo => repo.IncrementWonGames(accountId))
                .Returns(result);

            // Act
            var actualResult = _winsManager.IncrementWins(accountId);

            // Assert
            Assert.IsTrue(actualResult.IsSuccess);
            Assert.AreEqual("Win incremented successfully", actualResult.Value);
        }

        [TestMethod]
        public void IncrementWins_FailedIncrement_ReturnsFailure()
        {
            // Arrange
            int accountId = 1;
            var result = Result<string>.Failure("Failed to increment wins");

            // Setup mock to return failure result
            _gamesRepositoryMock.Setup(repo => repo.IncrementWonGames(accountId))
                .Returns(result);

            // Act
            var actualResult = _winsManager.IncrementWins(accountId);

            // Assert
            Assert.IsFalse(actualResult.IsSuccess);
            Assert.AreEqual("Failed to increment wins", actualResult.Error);
        }

        [TestMethod]
        public void IncrementDefeats_ValidAccountId_ReturnsSuccess()
        {
            // Arrange
            int accountId = 1;
            var result = Result<string>.Success("Defeat incremented successfully");

            // Setup mock to return success result
            _gamesRepositoryMock.Setup(repo => repo.IncrementDeafeatGames(accountId))
                .Returns(result);

            // Act
            var actualResult = _winsManager.IncrementDefeats(accountId);

            // Assert
            Assert.IsTrue(actualResult.IsSuccess);
            Assert.AreEqual("Defeat incremented successfully", actualResult.Value);
        }

        [TestMethod]
        public void IncrementDefeats_FailedIncrement_ReturnsFailure()
        {
            // Arrange
            int accountId = 1;
            var result = Result<string>.Failure("Failed to increment defeats");

            // Setup mock to return failure result
            _gamesRepositoryMock.Setup(repo => repo.IncrementDeafeatGames(accountId))
                .Returns(result);

            // Act
            var actualResult = _winsManager.IncrementDefeats(accountId);

            // Assert
            Assert.IsFalse(actualResult.IsSuccess);
            Assert.AreEqual("Failed to increment defeats", actualResult.Error);
        }
    }
}
