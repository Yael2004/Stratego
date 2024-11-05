using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Test.RepositoryTest
{
    [TestClass]
    public class GamesRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Lazy<StrategoEntities> _lazyMockContext;

        [TestInitialize]
        public void Initialize()
        {
            _mockContext = new Mock<StrategoEntities>();
            _lazyMockContext = new Lazy<StrategoEntities>(() => _mockContext.Object);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        [TestMethod]
        public async Task Test_GetGameStatisticsByAccountIdAsync_ShouldReturnGameStatistics_WhenStatisticsExist()
        {
            var repository = new GamesRepository(_lazyMockContext);
            int accountId = 1;

            var gameStatistics = new Games
            {
                AccountId = accountId,
                WonGames = 5,
                DeafeatGames = 2
            };

            var gamesData = new List<Games> { gameStatistics }.AsQueryable();
            var mockGamesSet = CreateMockDbSet(gamesData);

            _mockContext.Setup(c => c.Games).Returns(mockGamesSet.Object);

            var result = await repository.GetGameStatisticsByAccountIdAsync(accountId);

            Assert.AreEqual(gameStatistics, result.Value);
        }

        [TestMethod]
        public async Task Test_GetGameStatisticsByAccountIdAsync_ShouldReturnFailure_WhenStatisticsDoNotExist()
        {
            var repository = new GamesRepository(_lazyMockContext);
            int accountId = 2;

            var gamesData = new List<Games>().AsQueryable();
            var mockGamesSet = CreateMockDbSet(gamesData);

            _mockContext.Setup(c => c.Games).Returns(mockGamesSet.Object);

            var result = await repository.GetGameStatisticsByAccountIdAsync(accountId);

            Assert.AreEqual("Not available", result.Error);
        }

        [TestMethod]
        public async Task Test_GetGameStatisticsByAccountIdAsync_ShouldReturnDatabaseError_WhenExceptionIsThrown()
        {
            var repository = new GamesRepository(_lazyMockContext);
            int accountId = 1;

            var mockGamesSet = new Mock<DbSet<Games>>();
            _mockContext.Setup(c => c.Games).Returns(mockGamesSet.Object);
            _mockContext.Setup(c => c.Games.FirstOrDefault(It.IsAny<Func<Games, bool>>())).Throws(new Exception("Simulated database error"));

            var result = await repository.GetGameStatisticsByAccountIdAsync(accountId);

            Assert.IsFalse(result.IsSuccess);
        }
    }
}
