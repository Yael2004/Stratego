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

            _playerRepository = new PlayerRepository(new Lazy<StrategoEntities>(() => _mockContext.Object));
        }

        [TestMethod]
        public void Test_GetOtherPlayerById_ShouldReturnPlayer_WhenPlayerExists()
        {
            var playerId = 1;

            var result = _playerRepository.GetOtherPlayerById(playerId);

            Assert.AreEqual(playerId, result.Value.Id);
        }

        [TestMethod]
        public void Test_GetOtherPlayerById_ShouldReturnFailure_WhenPlayerDoesNotExist()
        {
            var playerId = 3;

            var result = _playerRepository.GetOtherPlayerById(playerId);

            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void Test_GetOtherPlayerById_ShouldHandleUnexpectedException()
        {
            var playerId = 1;
            _mockContext.Setup(c => c.Player).Throws(new Exception("Unexpected error"));

            var result = _playerRepository.GetOtherPlayerById(playerId);
            
            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }

        [TestMethod]
        public void Test_IsFriend_ShouldReturnTrue_WhenPlayersAreFriends()
        {
            var playerId = 1;
            var otherPlayerId = 2;

            var result = _playerRepository.IsFriend(playerId, otherPlayerId);

            Assert.IsTrue(result.Value);
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
        public void Test_IsFriend_ShouldHandleSqlException()
        {
            var playerId = 1;
            var otherPlayerId = 2;
            _mockContext.Setup(c => c.Friend).Throws(new InvalidOperationException("Simulated database error"));

            var result = _playerRepository.IsFriend(playerId, otherPlayerId);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void Test_GetPlayerFriendsList_ShouldReturnFriendsList_WhenFriendsExist()
        {
            var playerId = 1;

            _fakeFriendSet.Add(new Friend { PlayerId = playerId, FriendId = 2, Status = "accepted" });

            _fakePlayerSet.Add(new Player { Id = 2, Name = "FriendPlayer" });

            _mockContext.Setup(c => c.Friend).Returns(_fakeFriendSet);
            _mockContext.Setup(c => c.Player).Returns(_fakePlayerSet);

            var result = _playerRepository.GetPlayerFriendsList(playerId);

            Assert.AreEqual(2, result.Value.First().Id);
        }


        [TestMethod]
        public void Test_GetPlayerFriendsList_ShouldReturnFailure_WhenNoFriendsExist()
        {
            var playerId = 3;

            var result = _playerRepository.GetPlayerFriendsList(playerId);

            Assert.AreEqual("No friends found for the given player ID", result.Error);
        }

        [TestMethod]
        public void Test_GetPlayerFriendsList_ShouldHandleUnexpectedException()
        {
            var playerId = 1;
            _mockContext.Setup(c => c.Friend).Throws(new Exception("Unexpected error"));

            var result = _playerRepository.GetPlayerFriendsList(playerId);

            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }

        [TestMethod]
        public void Test_UpdatePlayer_ShouldReturnUpdatedPlayer_WhenUpdateIsSuccessful()
        {
            var updatedPlayer = new Player { Id = 1, Name = "UpdatedPlayer", AccountId = 1 };

            var existingPlayer = _fakePlayerSet.FirstOrDefault(p => p.Id == updatedPlayer.Id);
            if (existingPlayer == null)
            {
                _fakePlayerSet.Add(new Player { Id = 1, Name = "OriginalPlayer", AccountId = 1, PictureId = 1, IdLabel = 1 });
            }

            _mockContext.Setup(c => c.Pictures)
                .Returns(new FakeDbSet<Pictures> { new Pictures { IdPicture = 1, path = "picturePath" } });
            _mockContext.Setup(c => c.Label)
                .Returns(new FakeDbSet<Label> { new Label { IdLabel = 1, Path = "labelPath" } });

            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            var result = _playerRepository.UpdatePlayer(updatedPlayer, "labelPath", "picturePath");

            Assert.AreEqual("UpdatedPlayer", result.Value.Name);
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
