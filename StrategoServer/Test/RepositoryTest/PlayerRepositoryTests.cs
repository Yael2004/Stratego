using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using StrategoDataAccess;
using Utilities;
using System.Linq;
using System.Collections.Generic;
using Test.RepositoryTest;

namespace Tests
{
    [TestClass]
    public class PlayerRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private FakeDbSet<Player> _fakePlayerSet;
        private FakeDbSet<Friend> _fakeFriendSet;
        private PlayerRepository _playerRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();

            _fakePlayerSet = new FakeDbSet<Player>
            {
                new Player { Id = 1, Name = "Player1", AccountId = 1 },
                new Player { Id = 2, Name = "Player2", AccountId = 2 }
            };

            _fakeFriendSet = new FakeDbSet<Friend>
            {
                new Friend { PlayerId = 1, FriendId = 2, Status = "friend" }
            };

            _mockContext.Setup(c => c.Player).Returns(_fakePlayerSet);
            _mockContext.Setup(c => c.Friend).Returns(_fakeFriendSet);

            _playerRepository = new PlayerRepository();
        }

        [TestMethod]
        public void Test_GetOtherPlayerById_ShouldReturnFailure_WhenPlayerDoesNotExist()
        {
            var playerId = 180;

            var result = _playerRepository.GetOtherPlayerById(playerId);

            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void Test_IsFriend_ShouldReturnFalse_WhenPlayersAreNotFriends()
        {
            var playerId = 1;
            var otherPlayerId = 3;

            var result = _playerRepository.IsFriend(playerId, otherPlayerId);

            Assert.IsFalse(result.Value);
        }

        [TestMethod]
        public void Test_GetPlayerFriendsList_ShouldReturnFailure_WhenNoFriendsExist()
        {
            var playerId = 155;

            var result = _playerRepository.GetPlayerFriendsList(playerId);

            Assert.AreEqual("No friends found for the given player ID", result.Error);
        }

        [TestMethod]
        public void Test_UpdatePlayer_ShouldReturnFailure_WhenPlayerNotFound()
        {
            var updatedPlayer = new Player { Id = 3, Name = "NonExistentPlayer", AccountId = 3 };

            _mockContext.Setup(c => c.Pictures)
                .Returns(new FakeDbSet<Pictures> { new Pictures { IdPicture = 1, path = "picturePath" } });
            _mockContext.Setup(c => c.Label)
                .Returns(new FakeDbSet<Label> { new Label { IdLabel = 1, Path = "labelPath" } });

            var result = _playerRepository.UpdatePlayer(updatedPlayer, "labelPath", "picturePath");

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void Test_UpdatePlayer_ShouldHandleDbEntityValidationException()
        {
            var updatedPlayer = new Player { Id = 1, Name = "UpdatedPlayer", AccountId = 1 };
            _mockContext.Setup(c => c.SaveChanges()).Throws(new DbEntityValidationException("Validation error"));

            var result = _playerRepository.UpdatePlayer(updatedPlayer, "labelPath", "picturePath");

            Assert.IsFalse(result.IsSuccess);
        }
    }
}