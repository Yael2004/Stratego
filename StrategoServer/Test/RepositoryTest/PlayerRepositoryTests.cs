/*
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Test.RepositoryTest
{
    [TestClass]
    public class PlayerRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Lazy<StrategoEntities> _lazyMockContext;
        private Mock<DbSet<Player>> _mockPlayerSet;

        [TestInitialize]
        public void Initialize()
        {
            _mockContext = new Mock<StrategoEntities>();
            _lazyMockContext = new Lazy<StrategoEntities>(() => _mockContext.Object);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mockSet.As<IDbAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockSet;
        }

        [TestMethod]
        public async Task Test_GetOtherPlayerByIdAsync_ShouldReturnPlayerWhenFound()
        {
            var playerId = 1;
            var player = new Player { Id = playerId, Name = "Player1" };
            var players = new List<Player> { player };
            _mockPlayerSet = CreateMockDbSet(players);
            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.GetOtherPlayerByIdAsync(playerId);

            Assert.AreEqual("Player1", result.Value.Name);
        }

        [TestMethod]
        public async Task Test_GetOtherPlayerByIdAsync_ShouldReturnFailureWhenNotFound()
        {
            var players = new List<Player>(); 
            _mockPlayerSet = CreateMockDbSet(players);
            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.GetOtherPlayerByIdAsync(1);

            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public async Task Test_GetOtherPlayerByIdAsync_ShouldReturnFailureOnGeneralException()
        {
            _mockPlayerSet = new Mock<DbSet<Player>>();
            _mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.Provider)
                .Throws(new Exception("Unexpected error"));
            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.GetOtherPlayerByIdAsync(1);

            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }

        [TestMethod]
        public async Task Test_IsFriendAsync_ShouldReturnTrueWhenFriends()
        {
            var playerId = 1;
            var otherPlayerId = 2;
            var friendData = new List<Friend>
            {
                new Friend { PlayerId = playerId, FriendId = otherPlayerId, Status = "friend" }
            };
            var mockFriendSet = CreateMockDbSet(friendData);
            _mockContext.Setup(c => c.Friend).Returns(mockFriendSet.Object);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.IsFriendAsync(playerId, otherPlayerId);

            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task Test_GetPlayerByAccountIdAsync_ShouldReturnPlayerWhenFound()
        {
            var accountId = 100;
            var player = new Player { AccountId = accountId, Name = "John Doe" };
            _mockPlayerSet = CreateMockDbSet(new List<Player> { player });
            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.GetPlayerByAccountIdAsync(accountId);

            Assert.AreEqual("John Doe", result.Value.Name);
        }
    }
}
*/