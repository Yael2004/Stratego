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
            _gamesRepositoryMock = new Mock<GamesRepository>();
            _playerRepositoryMock = new Mock<PlayerRepository>();

            _profilesManager = new ProfilesManager(
                new Lazy<GamesRepository>(() => _gamesRepositoryMock.Object),
                new Lazy<PlayerRepository>(() => _playerRepositoryMock.Object)
            );
        }

        [TestMethod]
        public void Test_GetPlayerInfo_RepositoryFailure_ReturnsFailure()
        {
            int playerId = 1;
            int requesterId = 2;

            var playerResult = Result<Player>.Failure("Player not found");
            _playerRepositoryMock.Setup(repo => repo.GetOtherPlayerById(playerId)).Returns(playerResult);

            var result = _profilesManager.GetPlayerInfo(playerId, requesterId);

            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void Test_UpdatePlayerProfile_Success_ReturnsUpdatedPlayerInfo()
        {
            var playerInfo = new PlayerInfoShownDTO
            {
                Id = 1,
                Name = "UpdatedName",
                PicturePath = "new/path/to/picture",
                LabelPath = "new/path/to/label"
            };

            var player = new Player { Id = 1, Name = "UpdatedName" };
            var updateResult = Result<Player>.Success(player);

            _playerRepositoryMock.Setup(repo => repo.UpdatePlayer(It.IsAny<Player>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(updateResult);

            var result = _profilesManager.UpdatePlayerProfile(playerInfo);

            Assert.AreEqual("UpdatedName", result.Value.Name);
        }

        [TestMethod]
        public void Test_UpdatePlayerProfile_Failure_ReturnsFailure()
        {
            var playerInfo = new PlayerInfoShownDTO
            {
                Id = 1,
                Name = "UpdatedName",
                PicturePath = "new/path/to/picture",
                LabelPath = "new/path/to/label"
            };

            var updateResult = Result<Player>.Failure("Failed to update player");

            _playerRepositoryMock.Setup(repo => repo.UpdatePlayer(It.IsAny<Player>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(updateResult);

            var result = _profilesManager.UpdatePlayerProfile(playerInfo);

            Assert.AreEqual("Failed to update player", result.Error);
        }

        [TestMethod]
        public void Test_GetFriendIdsList_ValidPlayer_ReturnsFriendIds()
        {
            int playerId = 1;
            var friends = new List<Player> { new Player { Id = 2 }, new Player { Id = 3 } };
            var friendsResult = Result<IEnumerable<Player>>.Success(friends);

            _playerRepositoryMock.Setup(repo => repo.GetPlayerFriendsList(playerId)).Returns(friendsResult);

            var result = _profilesManager.GetFriendIdsList(playerId);

            CollectionAssert.AreEqual(new List<int> { 2, 3 }, result.Value);
        }

        [TestMethod]
        public void Test_GetFriendIdsList_NoFriends_ReturnsFailure()
        {
            int playerId = 1;
            var friendsResult = Result<IEnumerable<Player>>.Success(new List<Player>());

            _playerRepositoryMock.Setup(repo => repo.GetPlayerFriendsList(playerId)).Returns(friendsResult);

            var result = _profilesManager.GetFriendIdsList(playerId);

            Assert.AreEqual("No friends found.", result.Error);
        }

        [TestMethod]
        public void Test_GetTopPlayersIds_ValidRequest_ReturnsTopPlayersIds()
        {
            var topPlayers = new List<Player> { new Player { Id = 1 }, new Player { Id = 2 } };
            var topPlayersResult = Result<IEnumerable<Player>>.Success(topPlayers);
            var playerIds = topPlayersResult.Value.Select(player => player.Id).ToList();
            var resultWrapper = Result<List<int>>.Success(playerIds);
            _playerRepositoryMock.Setup(repo => repo.GetTopPlayersByWins()).Returns(resultWrapper);

            var result = _profilesManager.GetTopPlayersIds();

            CollectionAssert.AreEqual(new List<int> { 1, 2 }, result.Value);
        }

        [TestMethod]
        public void Test_GetTopPlayersIds_DatabaseError_ReturnsFailure()
        {
            var resultWrapper = Result<List<int>>.DataBaseError("Database error");
            _playerRepositoryMock.Setup(repo => repo.GetTopPlayersByWins()).Returns(resultWrapper);

            var result = _profilesManager.GetTopPlayersIds();
            Assert.AreEqual("Database error", result.Error);
        }
    }
}
