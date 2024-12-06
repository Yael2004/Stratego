using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using StrategoDataAccess;
using Utilities;
using System.Linq;
using System.Collections.Generic;
using Test.RepositoryTest;

namespace Tests
{
    [TestClass]
    public class GamesRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private FakeDbSet<Games> _fakeGamesSet;
        private GamesRepository _gamesRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();

            _fakeGamesSet = new FakeDbSet<Games>
            {
                new Games { AccountId = 1, WonGames = 10, DeafeatGames = 5 }
            };

            _mockContext.Setup(c => c.Games).Returns(_fakeGamesSet);

            _gamesRepository = new GamesRepository();
        }

        [TestMethod]
        public void Test_GetGameStatisticsByAccountId_ShouldReturnStatistics_WhenAccountIdExists()
        {
            var accountId = 1;

            var result = _gamesRepository.GetGameStatisticsByAccountId(accountId);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(accountId, result.Value.AccountId);
            Assert.AreEqual(10, result.Value.WonGames);
            Assert.AreEqual(5, result.Value.DeafeatGames);
        }

        [TestMethod]
        public void Test_GetGameStatisticsByAccountId_ShouldReturnFailure_WhenAccountIdDoesNotExist()
        {
            var accountId = 2;

            var result = _gamesRepository.GetGameStatisticsByAccountId(accountId);

            Assert.AreEqual("Not available", result.Error);
        }

        [TestMethod]
        public void Test_GetGameStatisticsByAccountId_ShouldHandleSqlException()
        {
            var accountId = 1;
            _mockContext.Setup(c => c.Games).Throws(new InvalidOperationException("Simulated database error"));

            var result = _gamesRepository.GetGameStatisticsByAccountId(accountId);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void Test_GetGameStatisticsByAccountId_ShouldHandleUnexpectedException()
        {
            var accountId = 1;
            _mockContext.Setup(c => c.Games).Throws(new Exception("Unexpected error"));

            var result = _gamesRepository.GetGameStatisticsByAccountId(accountId);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }
    }
}
