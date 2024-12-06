using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoServices.Logic;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace StrategoServices.Tests
{
    [TestClass]
    public class FriendsManagerTests
    {
        private FriendsManager _friendsManager;
        private Mock<FriendsRepository> _friendsRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            // Crear el Mock para el repositorio de amigos
            _friendsRepositoryMock = new Mock<FriendsRepository>();

            // Inicializar el FriendsManager con el Mock
            _friendsManager = new FriendsManager(new Lazy<FriendsRepository>(() => _friendsRepositoryMock.Object));
        }

        [TestMethod]
        public void SendFriendRequest_SamePlayerIds_ReturnsFailure()
        {
            // Arrange
            int destinationId = 1;
            int requesterId = 1;

            // Act
            var result = _friendsManager.SendFriendRequest(destinationId, requesterId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Cannot send a friend request to yourself.", result.Error);
        }

        [TestMethod]
        public void SendFriendRequest_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend request sent successfully.");

            // Configurar el comportamiento del Mock
            _friendsRepositoryMock.Setup(repo => repo.SendFriendRequest(destinationId, requesterId))
                .Returns(expectedResult);

            // Act
            var result = _friendsManager.SendFriendRequest(destinationId, requesterId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Friend request sent successfully.", result.Value);
        }

        [TestMethod]
        public void AcceptFriendRequest_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend request accepted.");

            // Configurar el comportamiento del Mock
            _friendsRepositoryMock.Setup(repo => repo.AcceptFriendRequest(destinationId, requesterId))
                .Returns(expectedResult);

            // Act
            var result = _friendsManager.AcceptFriendRequest(destinationId, requesterId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Friend request accepted.", result.Value);
        }

        [TestMethod]
        public void DeclineFriendRequest_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend request declined.");

            // Configurar el comportamiento del Mock
            _friendsRepositoryMock.Setup(repo => repo.DeclineFriendRequest(destinationId, requesterId))
                .Returns(expectedResult);

            // Act
            var result = _friendsManager.DeclineFriendRequest(destinationId, requesterId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Friend request declined.", result.Value);
        }

        [TestMethod]
        public void RemoveFriend_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend removed.");

            // Configurar el comportamiento del Mock
            _friendsRepositoryMock.Setup(repo => repo.RemoveFriend(destinationId, requesterId))
                .Returns(expectedResult);

            // Act
            var result = _friendsManager.RemoveFriend(destinationId, requesterId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Friend removed.", result.Value);
        }

        [TestMethod]
        public void GetFriendRequestsFromRepository_ReturnsFriendRequests()
        {
            // Arrange
            int playerId = 1;
            var playerList = new List<Player>
            {
                new Player { Id = 1, Name = "Player1" },
                new Player { Id = 2, Name = "Player2" }
            };
            var expectedResult = Result<IEnumerable<Player>>.Success(playerList);

            // Configurar el comportamiento del Mock
            _friendsRepositoryMock.Setup(repo => repo.GetPendingFriendRequests(playerId))
                .Returns(expectedResult);

            // Act
            var result = _friendsManager.GetFriendRequestsFromRepository(playerId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count());
            Assert.AreEqual("Player1", result.Value.First().Name);
        }

        [TestMethod]
        public void GetFriendRequestIdsList_NoFriendRequests_ReturnsFailure()
        {
            // Arrange
            int playerId = 1;
            var expectedResult = Result<IEnumerable<Player>>.Failure("No friend requests found.");

            // Configurar el comportamiento del Mock
            _friendsRepositoryMock.Setup(repo => repo.GetPendingFriendRequests(playerId))
                .Returns(expectedResult);

            // Act
            var result = _friendsManager.GetFriendRequestIdsList(playerId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No friend requests found.", result.Error);
        }

        [TestMethod]
        public void GetFriendRequestIdsList_ValidFriendRequests_ReturnsIdsList()
        {
            // Arrange
            int playerId = 1;
            var playerList = new List<Player>
            {
                new Player { Id = 1, Name = "Player1" },
                new Player { Id = 2, Name = "Player2" }
            };
            var expectedResult = Result<IEnumerable<Player>>.Success(playerList);

            // Configurar el comportamiento del Mock
            _friendsRepositoryMock.Setup(repo => repo.GetPendingFriendRequests(playerId))
                .Returns(expectedResult);

            // Act
            var result = _friendsManager.GetFriendRequestIdsList(playerId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count);
            Assert.AreEqual(1, result.Value[0]);
            Assert.AreEqual(2, result.Value[1]);
        }
    }
}
