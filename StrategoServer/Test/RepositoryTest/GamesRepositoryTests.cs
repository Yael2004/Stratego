using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace StrategoDataAccess.Tests
{
    [TestClass]
    public class GamesRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Mock<DbSet<Games>> _mockGamesDbSet;
        private GamesRepository _gamesRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();
            _mockGamesDbSet = new Mock<DbSet<Games>>();

            _mockContext.Setup(c => c.Games).Returns(_mockGamesDbSet.Object);

            _gamesRepository = new GamesRepository();
        }

        [TestMethod]
        public void Test_IncrementWonGames_ShouldReturnSuccess_WhenGameFound()
        {
            int accountId = 1;
            var game = new Games { AccountId = accountId, WonGames = 5, DeafeatGames = 3 };

            var gameList = new List<Games> { game }.AsQueryable();

            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Provider).Returns(gameList.Provider);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Expression).Returns(gameList.Expression);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.ElementType).Returns(gameList.ElementType);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.GetEnumerator()).Returns(gameList.GetEnumerator());

            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            var result = _gamesRepository.IncrementWonGames(accountId);

            Assert.AreEqual(5, game.WonGames);
        }

        [TestMethod]
        public void Test_GetGameStatisticsByAccountId_ShouldReturnFailure_WhenGameNotFound()
        {
            int accountId = 1;
            var gameList = new List<Games>().AsQueryable();

            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Provider).Returns(gameList.Provider);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Expression).Returns(gameList.Expression);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.ElementType).Returns(gameList.ElementType);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.GetEnumerator()).Returns(gameList.GetEnumerator());

            var result = _gamesRepository.GetGameStatisticsByAccountId(accountId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Not available", result.Error);
        }

        [TestMethod]
        public void Test_IncrementWonGames_ShouldReturnFailure_WhenGameNotFound()
        {
            int accountId = 1;
            var gameList = new List<Games>().AsQueryable();

            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Provider).Returns(gameList.Provider);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Expression).Returns(gameList.Expression);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.ElementType).Returns(gameList.ElementType);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.GetEnumerator()).Returns(gameList.GetEnumerator());

            var result = _gamesRepository.IncrementWonGames(accountId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Game record not found for the specified AccountId.", result.Error);
        }

        [TestMethod]
        public void Test_IncrementDeafeatGames_ShouldReturnFailure_WhenGameNotFound()
        {
            int accountId = 1;
            var gameList = new List<Games>().AsQueryable();

            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Provider).Returns(gameList.Provider);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.Expression).Returns(gameList.Expression);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.ElementType).Returns(gameList.ElementType);
            _mockGamesDbSet.As<IQueryable<Games>>().Setup(m => m.GetEnumerator()).Returns(gameList.GetEnumerator());

            var result = _gamesRepository.IncrementDeafeatGames(accountId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Game record not found for the specified AccountId.", result.Error);
        }
    }
}
