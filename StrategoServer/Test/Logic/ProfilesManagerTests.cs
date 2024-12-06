using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoServices.Logic;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using StrategoServices.Data.DTO;

namespace StrategoServices.Tests
{
    [TestClass]
    public class ProfilesManagerTests
    {
        private ProfilesManager _profilesManager;
        private Mock<GamesRepository> _gamesRepositoryMock;
        private Mock<PlayerRepository> _playerRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            // Crear los Mocks para los repositorios
            _gamesRepositoryMock = new Mock<GamesRepository>();
            _playerRepositoryMock = new Mock<PlayerRepository>();

            // Inicializar el ProfilesManager con los Mocks
            _profilesManager = new ProfilesManager(
                new Lazy<GamesRepository>(() => _gamesRepositoryMock.Object),
                new Lazy<PlayerRepository>(() => _playerRepositoryMock.Object)
            );
        }

        [TestMethod]
        public void GetPlayerInfo_RepositoryFailure_ReturnsFailure()
        {
            // Arrange
            int playerId = 1;
            int requesterId = 2;

            var playerResult = Result<Player>.Failure("Player not found");
            _playerRepositoryMock.Setup(repo => repo.GetOtherPlayerById(playerId)).Returns(playerResult);

            // Act
            var result = _profilesManager.GetPlayerInfo(playerId, requesterId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void UpdatePlayerProfile_Success_ReturnsUpdatedPlayerInfo()
        {
            // Arrange
            var playerInfo = new PlayerInfoShownDTO
            {
                Id = 1,
                Name = "UpdatedName",
                PicturePath = "new/path/to/picture",
                LabelPath = "new/path/to/label"
            };

            var player = new Player { Id = 1, Name = "UpdatedName" };
            var updateResult = Result<Player>.Success(player);

            // Configurar el Mock para la actualización
            _playerRepositoryMock.Setup(repo => repo.UpdatePlayer(It.IsAny<Player>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(updateResult);

            // Act
            var result = _profilesManager.UpdatePlayerProfile(playerInfo);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("UpdatedName", result.Value.Name);
            Assert.AreEqual("new/path/to/picture", result.Value.PicturePath);
            Assert.AreEqual("new/path/to/label", result.Value.LabelPath);
        }

        [TestMethod]
        public void UpdatePlayerProfile_Failure_ReturnsFailure()
        {
            // Arrange
            var playerInfo = new PlayerInfoShownDTO
            {
                Id = 1,
                Name = "UpdatedName",
                PicturePath = "new/path/to/picture",
                LabelPath = "new/path/to/label"
            };

            var updateResult = Result<Player>.Failure("Failed to update player");

            // Configurar el Mock para la actualización
            _playerRepositoryMock.Setup(repo => repo.UpdatePlayer(It.IsAny<Player>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(updateResult);

            // Act
            var result = _profilesManager.UpdatePlayerProfile(playerInfo);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to update player", result.Error);
        }

        [TestMethod]
        public void GetFriendIdsList_ValidPlayer_ReturnsFriendIds()
        {
            // Arrange
            int playerId = 1;
            var friends = new List<Player> { new Player { Id = 2 }, new Player { Id = 3 } };
            var friendsResult = Result<IEnumerable<Player>>.Success(friends);

            // Configurar el Mock para obtener los amigos
            _playerRepositoryMock.Setup(repo => repo.GetPlayerFriendsList(playerId)).Returns(friendsResult);

            // Act
            var result = _profilesManager.GetFriendIdsList(playerId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, result.Value);
        }

        [TestMethod]
        public void GetFriendIdsList_NoFriends_ReturnsFailure()
        {
            // Arrange
            int playerId = 1;
            var friendsResult = Result<IEnumerable<Player>>.Success(new List<Player>());

            // Configurar el Mock para obtener los amigos
            _playerRepositoryMock.Setup(repo => repo.GetPlayerFriendsList(playerId)).Returns(friendsResult);

            // Act
            var result = _profilesManager.GetFriendIdsList(playerId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No friends found.", result.Error);
        }

        [TestMethod]
        public void GetTopPlayersIds_ValidRequest_ReturnsTopPlayersIds()
        {
            // Arrange
            var topPlayers = new List<Player> { new Player { Id = 1 }, new Player { Id = 2 } };
            var topPlayersResult = Result<IEnumerable<Player>>.Success(topPlayers);

            // Configurar el Mock para obtener los jugadores top
            _playerRepositoryMock.Setup(repo => repo.GetTopPlayersByWins()).Returns(topPlayersResult);

            // Act
            var result = _profilesManager.GetTopPlayersIds();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            CollectionAssert.AreEqual(new List<int> { 1, 2 }, result.Value);
        }

        [TestMethod]
        public void GetTopPlayersIds_NoTopPlayers_ReturnsFailure()
        {
            // Arrange
            var topPlayersResult = Result<IEnumerable<Player>>.Success(new List<Player>());

            // Configurar el Mock para obtener los jugadores top
            _playerRepositoryMock.Setup(repo => repo.GetTopPlayersByWins()).Returns(topPlayersResult);

            // Act
            var result = _profilesManager.GetTopPlayersIds();

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No top players found.", result.Error);
        }
    }
}
