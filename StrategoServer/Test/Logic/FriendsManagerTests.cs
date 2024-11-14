using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using StrategoServices.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Test.Logic
{
    [TestClass]
    public class FriendsManagerTests
    {
        private Mock<FriendsRepository> _mockFriendsRepository;
        private FriendsManager _friendsManager;

        [TestInitialize]
        public void Setup()
        {
            _mockFriendsRepository = new Mock<FriendsRepository>(new Lazy<StrategoEntities>());
            _friendsManager = new FriendsManager(new Lazy<FriendsRepository>(() => _mockFriendsRepository.Object));
        }

        [TestMethod]
        public void SendFriendRequest_ShouldReturnFailure_WhenRequestingSelf()
        {
            var result = _friendsManager.SendFriendRequest(1, 1);
            Assert.AreEqual("Cannot send a friend request to yourself.", result.Error);
        }

        [TestMethod]
        public void SendFriendRequest_ShouldReturnSuccess_WhenRequestIsSent()
        {
            _mockFriendsRepository.Setup(repo => repo.SendFriendRequest(2, 1))
                .Returns(Result<string>.Success("Friend request sent successfully."));

            var result = _friendsManager.SendFriendRequest(2, 1);
            Assert.AreEqual("Friend request sent successfully.", result.Value);
        }

        [TestMethod]
        public void SendFriendRequest_ShouldReturnFailure_WhenRequestAlreadyExists()
        {
            _mockFriendsRepository.Setup(repo => repo.SendFriendRequest(2, 1))
                .Returns(Result<string>.Failure("Friend request already exists or players are already friends."));

            var result = _friendsManager.SendFriendRequest(2, 1);
            Assert.AreEqual("Friend request already exists or players are already friends.", result.Error);
        }

        [TestMethod]
        public void AcceptFriendRequest_ShouldReturnSuccess_WhenRequestIsAccepted()
        {
            _mockFriendsRepository.Setup(repo => repo.AcceptFriendRequest(2, 1))
                .Returns(Result<string>.Success("Friend request accepted successfully."));

            var result = _friendsManager.AcceptFriendRequest(2, 1);
            Assert.AreEqual("Friend request accepted successfully.", result.Value);
        }

        [TestMethod]
        public void AcceptFriendRequest_ShouldReturnFailure_WhenRequestNotFound()
        {
            _mockFriendsRepository.Setup(repo => repo.AcceptFriendRequest(2, 1))
                .Returns(Result<string>.Failure("Friend request not found."));

            var result = _friendsManager.AcceptFriendRequest(2, 1);
            Assert.AreEqual("Friend request not found.", result.Error);
        }

        [TestMethod]
        public void DeclineFriendRequest_ShouldReturnSuccess_WhenRequestIsDeclined()
        {
            _mockFriendsRepository.Setup(repo => repo.DeclineFriendRequest(2, 1))
                .Returns(Result<string>.Success("Friend request declined successfully."));

            var result = _friendsManager.DeclineFriendRequest(2, 1);
            Assert.AreEqual("Friend request declined successfully.", result.Value);
        }

        [TestMethod]
        public void DeclineFriendRequest_ShouldReturnFailure_WhenRequestNotFound()
        {
            _mockFriendsRepository.Setup(repo => repo.DeclineFriendRequest(2, 1))
                .Returns(Result<string>.Failure("Friend request not found."));

            var result = _friendsManager.DeclineFriendRequest(2, 1);
            Assert.AreEqual("Friend request not found.", result.Error);
        }

        [TestMethod]
        public void RemoveFriend_ShouldReturnSuccess_WhenFriendIsRemoved()
        {
            _mockFriendsRepository.Setup(repo => repo.RemoveFriend(2, 1))
                .Returns(Result<string>.Success("Friend removed successfully."));

            var result = _friendsManager.RemoveFriend(2, 1);
            Assert.AreEqual("Friend removed successfully.", result.Value);
        }

        [TestMethod]
        public void RemoveFriend_ShouldReturnFailure_WhenFriendshipNotFound()
        {
            _mockFriendsRepository.Setup(repo => repo.RemoveFriend(2, 1))
                .Returns(Result<string>.Failure("Friendship not found."));

            var result = _friendsManager.RemoveFriend(2, 1);
            Assert.AreEqual("Friendship not found.", result.Error);
        }

        [TestMethod]
        public void GetFriendRequestsFromRepository_ShouldReturnSuccess_WhenRequestsExist()
        {
            var pendingRequests = new List<Player> { new Player { Id = 2, Name = "FriendPlayer" } };
            _mockFriendsRepository.Setup(repo => repo.GetPendingFriendRequests(1))
                .Returns(Result<IEnumerable<Player>>.Success(pendingRequests));

            var result = _friendsManager.GetFriendRequestsFromRepository(1);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void GetFriendRequestsFromRepository_ShouldReturnFailure_WhenNoRequestsExist()
        {
            _mockFriendsRepository.Setup(repo => repo.GetPendingFriendRequests(1))
                .Returns(Result<IEnumerable<Player>>.Failure("No pending friend requests found."));

            var result = _friendsManager.GetFriendRequestsFromRepository(1);
            Assert.AreEqual("No pending friend requests found.", result.Error);
        }

        [TestMethod]
        public void GetFriendRequestIdsList_ShouldReturnIdsList_WhenRequestsExist()
        {
            var pendingRequests = new List<Player> { new Player { Id = 2 }, new Player { Id = 3 } };
            _mockFriendsRepository.Setup(repo => repo.GetPendingFriendRequests(1))
                .Returns(Result<IEnumerable<Player>>.Success(pendingRequests));

            var result = _friendsManager.GetFriendRequestIdsList(1);
            Assert.AreEqual(2, result.Value.Count);
        }

        [TestMethod]
        public void GetFriendRequestIdsList_ShouldReturnFailure_WhenNoRequestsExist()
        {
            _mockFriendsRepository.Setup(repo => repo.GetPendingFriendRequests(1))
                .Returns(Result<IEnumerable<Player>>.Failure("No pending friend requests found."));

            var result = _friendsManager.GetFriendRequestIdsList(1);
            Assert.AreEqual("No pending friend requests found.", result.Error);
        }
    }
}
