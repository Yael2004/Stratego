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

namespace Test.RepositoryTest
{
    [TestClass]
    public class PlayerRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Mock<DbSet<Player>> _mockPlayerSet;

        [TestInitialize]
        public void Initialize()
        {
            _mockContext = new Mock<StrategoEntities>();
            _mockPlayerSet = new Mock<DbSet<Player>>();

            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);
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
            // Arrange
            var repository = new PlayerRepository(_mockContext.Object);
            var playerId = 1;
            var player = new Player { Id = playerId, Name = "John Doe" };

            var mockPlayerSet = CreateMockDbSet(new List<Player> { player });
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            // Act
            var result = await repository.GetPlayerByIdAsync(playerId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(player, result.Value);
        }

        [TestMethod]
        public async Task GetPlayerByIdAsync_ShouldReturnFailureWhenNotFound()
        {
            // Arrange
            var repository = new PlayerRepository(_mockContext.Object);
            var playerId = 1;

            var mockPlayerSet = CreateMockDbSet(new List<Player>()); // Lista vacía, no se encuentra ningún jugador
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            // Act
            var result = await repository.GetPlayerByIdAsync(playerId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public async Task GetPlayerByIdAsync_ShouldReturnFailureOnSqlException()
        {
            // Arrange
            var repository = new PlayerRepository(_mockContext.Object);
            var playerId = 1;

            // Simular una excepción de base de datos
            var mockPlayerSet = new Mock<DbSet<Player>>();
            mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.Provider).Throws(new InvalidOperationException("Database error"));

            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            // Act
            var result = await repository.GetPlayerByIdAsync(playerId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Contains("Database error"));
        }

        [TestMethod]
        public async Task GetAllPlayersAsync_ShouldReturnAllPlayers()
        {
            // Arrange
            var repository = new PlayerRepository(_mockContext.Object);
            var players = new List<Player>
            {
                new Player { Id = 1, Name = "John Doe" },
                new Player { Id = 2, Name = "Jane Doe" }
            };

            var mockPlayerSet = CreateMockDbSet(players);
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            // Act
            var result = await repository.GetAllPlayersAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("John Doe", result.First().Name);
        }

        [TestMethod]
        public async Task UpdatePlayerAsync_ShouldUpdatePlayerNameWhenPlayerExists()
        {
            // Arrange
            var repository = new PlayerRepository(_mockContext.Object);
            var playerId = 1;
            var player = new Player { Id = playerId, Name = "Old Name" };

            var mockPlayerSet = CreateMockDbSet(new List<Player> { player });
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            // Act
            var result = await repository.UpdatePlayerAsync(playerId, "New Name");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("New Name", player.Name);
        }

        [TestMethod]
        public async Task UpdatePlayerAsync_ShouldReturnFalseWhenPlayerNotFound()
        {
            // Arrange
            var repository = new PlayerRepository(_mockContext.Object);
            var playerId = 1;

            var mockPlayerSet = CreateMockDbSet(new List<Player>()); // Lista vacía, no se encuentra ningún jugador
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            // Act
            var result = await repository.UpdatePlayerAsync(playerId, "New Name");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetPlayerByAccountIdAsync_ShouldReturnPlayerWhenFound()
        {
            // Arrange
            var repository = new PlayerRepository(_mockContext.Object);
            var accountId = 1;
            var player = new Player { AccountId = accountId, Name = "John Doe" };

            var mockPlayerSet = CreateMockDbSet(new List<Player> { player });
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            // Act
            var result = await repository.GetPlayerByAccountIdAsync(accountId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(player, result.Value);
        }


    }

}
