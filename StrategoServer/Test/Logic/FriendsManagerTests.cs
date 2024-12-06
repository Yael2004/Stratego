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
            _friendsRepositoryMock = new Mock<FriendsRepository>();

            _friendsManager = new FriendsManager(new Lazy<FriendsRepository>(() => _friendsRepositoryMock.Object));
        }

        [TestMethod]
        public void Test_SendFriendRequest_SamePlayerIds_ReturnsFailure()
        {
            int destinationId = 1;
            int requesterId = 1;

            var result = _friendsManager.SendFriendRequest(destinationId, requesterId);

            Assert.AreEqual("Cannot send a friend request to yourself.", result.Error);
        }

        [TestMethod]
        public void Test_SendFriendRequest_ValidRequest_ReturnsSuccess()
        {
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend request sent successfully.");

            _friendsRepositoryMock.Setup(repo => repo.SendFriendRequest(destinationId, requesterId))
                .Returns(expectedResult);

            var result = _friendsManager.SendFriendRequest(destinationId, requesterId);

            Assert.AreEqual("Friend request sent successfully.", result.Value);
        }

        [TestMethod]
        public void Test_AcceptFriendRequest_ValidRequest_ReturnsSuccess()
        {
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend request accepted.");

            _friendsRepositoryMock.Setup(repo => repo.AcceptFriendRequest(destinationId, requesterId))
                .Returns(expectedResult);

            var result = _friendsManager.AcceptFriendRequest(destinationId, requesterId);

            Assert.AreEqual("Friend request accepted.", result.Value);
        }

        [TestMethod]
        public void Test_DeclineFriendRequest_ValidRequest_ReturnsSuccess()
        {
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend request declined.");

            _friendsRepositoryMock.Setup(repo => repo.DeclineFriendRequest(destinationId, requesterId))
                .Returns(expectedResult);

            var result = _friendsManager.DeclineFriendRequest(destinationId, requesterId);

            Assert.AreEqual("Friend request declined.", result.Value);
        }

        [TestMethod]
        public void Test_RemoveFriend_ValidRequest_ReturnsSuccess()
        {
            int destinationId = 2;
            int requesterId = 1;
            var expectedResult = Result<string>.Success("Friend removed.");

            _friendsRepositoryMock.Setup(repo => repo.RemoveFriend(destinationId, requesterId))
                .Returns(expectedResult);

            var result = _friendsManager.RemoveFriend(destinationId, requesterId);

            Assert.AreEqual("Friend removed.", result.Value);
        }

        [TestMethod]
        public void Test_GetFriendRequestsFromRepository_ReturnsFriendRequests()
        {
            int playerId = 1;
            var playerList = new List<Player>
            {
                new Player { Id = 1, Name = "Player1" },
                new Player { Id = 2, Name = "Player2" }
            };
            var expectedResult = Result<IEnumerable<Player>>.Success(playerList);

            _friendsRepositoryMock.Setup(repo => repo.GetPendingFriendRequests(playerId))
                .Returns(expectedResult);

            var result = _friendsManager.GetFriendRequestsFromRepository(playerId);

            Assert.AreEqual(2, result.Value.Count());
        }

        [TestMethod]
        public void Test_GetFriendRequestIdsList_NoFriendRequests_ReturnsFailure()
        {
            int playerId = 1;
            var expectedResult = Result<IEnumerable<Player>>.Failure("No friend requests found.");

            _friendsRepositoryMock.Setup(repo => repo.GetPendingFriendRequests(playerId))
                .Returns(expectedResult);

            var result = _friendsManager.GetFriendRequestIdsList(playerId);

            Assert.AreEqual("No friend requests found.", result.Error);
        }

        [TestMethod]
        public void Test_GetFriendRequestIdsList_ValidFriendRequests_ReturnsIdsList()
        {
            int playerId = 1;
            var playerList = new List<Player>
            {
                new Player { Id = 1, Name = "Player1" },
                new Player { Id = 2, Name = "Player2" }
            };
            var expectedResult = Result<IEnumerable<Player>>.Success(playerList);

            _friendsRepositoryMock.Setup(repo => repo.GetPendingFriendRequests(playerId))
                .Returns(expectedResult);

            var result = _friendsManager.GetFriendRequestIdsList(playerId);

            Assert.AreEqual(2, result.Value.Count);
        }
    }
}
