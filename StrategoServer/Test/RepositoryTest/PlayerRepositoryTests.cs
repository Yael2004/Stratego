using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Test.RepositoryTest
{
    /*
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

            var players = new List<Player>
            {
                new Player { Id = 1, Name = "Player1", AccountId = 100 },
                new Player { Id = 2, Name = "Player2", AccountId = 101 }
            };

            _mockPlayerSet = CreateMockDbSet(players);

            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);

            _lazyMockContext = new Lazy<StrategoEntities>(() => _mockContext.Object);
        }


        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.As<IDbAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockSet;
        }

        [TestMethod]
        public async Task GetPlayerByIdAsync_ShouldReturnPlayerWhenFound()
        {
            var repository = new PlayerRepository(_lazyMockContext);
            var playerId = 1;
            var player = new Player { Id = playerId, Name = "John Doe" };

            var mockPlayerSet = CreateMockDbSet(new List<Player> { player });
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var result = await repository.GetPlayerByIdAsync(playerId);

            Assert.AreEqual(player, result.Value);
        }

        [TestMethod]
        public async Task GetPlayerByIdAsync_ShouldReturnFailureWhenNotFound()
        {
            var repository = new PlayerRepository(_lazyMockContext);
            var playerId = 1;

            var mockPlayerSet = CreateMockDbSet(new List<Player>()); 
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var result = await repository.GetPlayerByIdAsync(playerId);

            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public async Task GetPlayerByIdAsync_ShouldReturnFailureOnSqlException()
        {
            var repository = new PlayerRepository(_lazyMockContext);
            var playerId = 1;

            var mockPlayerSet = new Mock<DbSet<Player>>();
            mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.Provider).Throws(new InvalidOperationException("Database error"));

            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var result = await repository.GetPlayerByIdAsync(playerId);

            Assert.IsTrue(result.Error.Contains("Database error"));
        }

        [TestMethod]
        public async Task GetAllPlayersAsync_ShouldReturnAllPlayers()
        {
            var repository = new PlayerRepository(_lazyMockContext);
            var players = new List<Player>
            {
                new Player { Id = 1, Name = "John Doe" },
                new Player { Id = 2, Name = "Jane Doe" }
            };

            var mockPlayerSet = CreateMockDbSet(players);
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var result = await repository.GetAllPlayersAsync();

            Assert.AreEqual("John Doe", result.First().Name);
        }

        [TestMethod]
        public async Task UpdatePlayerAsync_ShouldUpdatePlayerNameWhenPlayerExists()
        {
            var repository = new PlayerRepository(_lazyMockContext);
            var playerId = 1;
            var player = new Player { Id = playerId, Name = "Old Name" };

            var mockPlayerSet = CreateMockDbSet(new List<Player> { player });
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var result = await repository.UpdatePlayerAsync(playerId, "New Name");

            Assert.AreEqual("New Name", player.Name);
        }

        [TestMethod]
        public async Task UpdatePlayerAsync_ShouldReturnFalseWhenPlayerNotFound()
        {
            var repository = new PlayerRepository(_lazyMockContext);
            var playerId = 1;

            var mockPlayerSet = CreateMockDbSet(new List<Player>()); 
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var result = await repository.UpdatePlayerAsync(playerId, "New Name");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetPlayerByAccountIdAsync_ShouldReturnPlayerWhenFound()
        {
            var repository = new PlayerRepository(_lazyMockContext);
            var accountId = 1;
            var player = new Player { AccountId = accountId, Name = "John Doe" };

            var mockPlayerSet = CreateMockDbSet(new List<Player> { player });
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var result = await repository.GetPlayerByAccountIdAsync(accountId);

            Assert.AreEqual(player, result.Value);
        }

        [TestMethod]
        public async Task GetPlayerByAccountIdAsync_ShouldReturnFailureWhenPlayerNotFound()
        {
            var accountId = 123;
            var mockPlayerSet = CreateMockDbSet(new List<Player>()); 
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.GetPlayerByAccountIdAsync(accountId);

            Assert.AreEqual("Player not found for the given account ID", result.Error);
        }

        [TestMethod]
        public async Task GetPlayerByAccountIdAsync_ShouldReturnFailureOnGeneralException()
        {
            var accountId = 123;

            var mockPlayerSet = new Mock<DbSet<Player>>();

            mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.Provider)
                .Throws(new Exception("General exception"));

            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.GetPlayerByAccountIdAsync(accountId);

            Assert.IsTrue(result.Error.Contains("Unexpected error: General exception"));  
        }

        [TestMethod]
        public async Task UpdatePlayerAsync_ShouldUpdatePlayerNameWithoutTransactionMock()
        {
            var playerId = 123;
            var newName = "UpdatedName";

            var player = new Player { Id = playerId, Name = "OldName" }; 
            var players = new List<Player> { player };  

            var mockPlayerSet = CreateMockDbSet(players);  
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync()).ReturnsAsync(1);

            var repository = new PlayerRepository(_lazyMockContext);

            var result = await repository.UpdatePlayerAsync(playerId, newName);

            Assert.AreEqual(newName, player.Name);  
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Once);  
        }
        

    }
    */
}
