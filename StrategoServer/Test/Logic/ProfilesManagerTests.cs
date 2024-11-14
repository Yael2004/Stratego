using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Logic
{
    [TestClass]
    public class ProfilesManagerTests
    {
        private Mock<GamesRepository> _mockGamesRepository;
        private Mock<PlayerRepository> _mockPlayerRepository;
        private ProfilesManager _profilesManager;

        [TestInitialize]
        public void Setup()
        {
            _mockGamesRepository = new Mock<GamesRepository>(new Lazy<StrategoEntities>());
            _mockPlayerRepository = new Mock<PlayerRepository>(new Lazy<StrategoEntities>());
            _profilesManager = new ProfilesManager(new Lazy<GamesRepository>(() => _mockGamesRepository.Object),
                                                   new Lazy<PlayerRepository>(() => _mockPlayerRepository.Object));
        }

        private void SetupCompletePlayerInfoMocks(Player player)
        {
            var picturePath = "path/to/picture";
            var labelPath = "path/to/label";

            _mockPlayerRepository.Setup(repo => repo.GetOtherPlayerById(player.Id)).Returns(Result<Player>.Success(player));
            _mockPlayerRepository.Setup(repo => repo.GetPicturePathById(player.PictureId)).Returns(Result<string>.Success(picturePath));
            _mockPlayerRepository.Setup(repo => repo.GetLabelPathById(player.IdLabel)).Returns(Result<string>.Success(labelPath));
            _mockGamesRepository.Setup(repo => repo.GetGameStatisticsByAccountId(player.AccountId))
                .Returns(Result<Games>.Success(new Games { WonGames = 5, DeafeatGames = 2 }));
            _mockPlayerRepository.Setup(repo => repo.IsFriend(2, player.Id)).Returns(Result<bool>.Success(true));
        }

        [TestMethod]
        public void GetPlayerGameStatistics_ShouldReturnSuccess_WhenStatisticsExist()
        {
            _mockGamesRepository.Setup(repo => repo.GetGameStatisticsByAccountId(1))
                .Returns(Result<Games>.Success(new Games { WonGames = 5, DeafeatGames = 3 }));

            var result = _profilesManager.GetPlayerGameStatistics(1);

            Assert.AreEqual(5, result.Value.WonGames);
            Assert.AreEqual(3, result.Value.LostGames);
        }

        [TestMethod]
        public void GetPlayerGameStatistics_ShouldReturnFailure_WhenStatisticsNotFound()
        {
            _mockGamesRepository.Setup(repo => repo.GetGameStatisticsByAccountId(1))
                .Returns(Result<Games>.Failure("Statistics not found"));

            var result = _profilesManager.GetPlayerGameStatistics(1);

            Assert.AreEqual("Statistics not found", result.Error);
        }


        [TestMethod]
        public void GetPlayerInfo_ShouldReturnSuccessResult_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.IsTrue(result.IsSuccess, "Expected success result for complete player info");
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnNotNullPlayerInfo_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.IsNotNull(result.Value.PlayerInfo, "PlayerInfo should not be null");
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnCorrectPlayerName_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.AreEqual("Player1", result.Value.PlayerInfo.Name);
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnCorrectPicturePath_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.AreEqual("path/to/picture", result.Value.PlayerInfo.PicturePath);
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnCorrectLabelPath_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.AreEqual("path/to/label", result.Value.PlayerInfo.LabelPath);
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnNotNullPlayerStatistics_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.IsNotNull(result.Value.PlayerStatistics, "PlayerStatistics should not be null");
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnCorrectWonGamesCount_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.AreEqual(5, result.Value.PlayerStatistics.WonGames);
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnCorrectLostGamesCount_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.AreEqual(2, result.Value.PlayerStatistics.LostGames);
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnIsFriendTrue_WhenPlayerInfoIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 2, IdLabel = 3, AccountId = 1 };
            SetupCompletePlayerInfoMocks(player);

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.IsTrue(result.Value.IsFriend, "Expected IsFriend to be true");
        }

        [TestMethod]
        public void GetPlayerInfo_ShouldReturnFailure_WhenPlayerNotFound()
        {
            _mockPlayerRepository.Setup(repo => repo.GetOtherPlayerById(1))
                .Returns(Result<Player>.Failure("Player not found"));

            var result = _profilesManager.GetPlayerInfo(1, 2);

            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void UpdatePlayerProfile_ShouldReturnSuccess_WhenUpdateIsSuccessful()
        {
            var playerDto = new PlayerInfoShownDTO { Id = 1, Name = "UpdatedPlayer", LabelPath = "new/label", PicturePath = "new/picture" };
            var updatedPlayer = new Player { Id = 1, Name = "UpdatedPlayer" };

            _mockPlayerRepository.Setup(repo => repo.UpdatePlayer(It.IsAny<Player>(), "new/label", "new/picture"))
                .Returns(Result<Player>.Success(updatedPlayer));

            var result = _profilesManager.UpdatePlayerProfile(playerDto);

            Assert.AreEqual("UpdatedPlayer", result.Value.Name);
            Assert.AreEqual("new/label", result.Value.LabelPath);
            Assert.AreEqual("new/picture", result.Value.PicturePath);
        }

        [TestMethod]
        public void UpdatePlayerProfile_ShouldReturnFailure_WhenUpdateFails()
        {
            var playerDto = new PlayerInfoShownDTO { Id = 1, Name = "UpdatedPlayer", LabelPath = "new/label", PicturePath = "new/picture" };

            _mockPlayerRepository.Setup(repo => repo.UpdatePlayer(It.IsAny<Player>(), "new/label", "new/picture"))
                .Returns(Result<Player>.Failure("Update failed"));

            var result = _profilesManager.UpdatePlayerProfile(playerDto);

            Assert.AreEqual("Update failed", result.Error);
        }


        [TestMethod]
        public void GetFriendIdsList_ShouldReturnFriendIds_WhenFriendsExist()
        {
            var friends = new List<Player> { new Player { Id = 2 }, new Player { Id = 3 } };
            _mockPlayerRepository.Setup(repo => repo.GetPlayerFriendsList(1)).Returns(Result<IEnumerable<Player>>.Success(friends));

            var result = _profilesManager.GetFriendIdsList(1);

            CollectionAssert.AreEqual(new List<int> { 2, 3 }, result.Value);
        }

        [TestMethod]
        public void GetFriendIdsList_ShouldReturnFailure_WhenNoFriendsExist()
        {
            _mockPlayerRepository.Setup(repo => repo.GetPlayerFriendsList(1))
                .Returns(Result<IEnumerable<Player>>.Failure("No friends found"));

            var result = _profilesManager.GetFriendIdsList(1);

            Assert.AreEqual("No friends found", result.Error);
        }
    }
}
