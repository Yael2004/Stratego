using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.RepositoryTest
{
    [TestClass]
    public class FriendsRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private FakeDbSet<Friend> _fakeFriendSet;
        private FakeDbSet<Player> _fakePlayerSet;
        private FriendsRepository _friendsRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();

            _fakeFriendSet = new FakeDbSet<Friend>
            {
                new Friend { PlayerId = 1, FriendId = 2, Status = "sent" }
            };

            _fakePlayerSet = new FakeDbSet<Player>
            {
                new Player { Id = 1, Name = "Player1" },
                new Player { Id = 2, Name = "Player2" }
            };

            _mockContext.Setup(c => c.Friend).Returns(_fakeFriendSet);
            _mockContext.Setup(c => c.Player).Returns(_fakePlayerSet);

            _friendsRepository = new FriendsRepository(new Lazy<StrategoEntities>(() => _mockContext.Object));
        }

        [TestMethod]
        public void Test_SendFriendRequest_ShouldReturnSuccess_WhenRequestIsSent()
        {
            var result = _friendsRepository.SendFriendRequest(2, 1);

            Assert.AreEqual("Friend request sent successfully.", result.Value);
        }

        [TestMethod]
        public void Test_SendFriendRequest_ShouldReturnFailure_WhenRequestAlreadyExists()
        {
            _fakeFriendSet.Add(new Friend { PlayerId = 1, FriendId = 2, Status = "accepted" });

            var result = _friendsRepository.SendFriendRequest(2, 1);

            Assert.AreEqual("Friend request already exists or players are already friends.", result.Error);
        }

        [TestMethod]
        public void Test_SendFriendRequest_ShouldHandleSqlException()
        {
            _mockContext.Setup(c => c.Friend).Throws(new InvalidOperationException("Database error"));

            var result = _friendsRepository.SendFriendRequest(2, 1);

            Assert.IsTrue(result.Error.Contains("Database error"));
        }

        [TestMethod]
        public void Test_AcceptFriendRequest_ShouldReturnSuccess_WhenRequestIsAccepted()
        {
            _fakeFriendSet.FirstOrDefault(f => f.PlayerId == 1 && f.FriendId == 2).Status = "sent";

            var result = _friendsRepository.AcceptFriendRequest(1, 2);

            Assert.AreEqual("Friend request accepted successfully.", result.Value);
        }

        [TestMethod]
        public void Test_AcceptFriendRequest_ShouldReturnFailure_WhenRequestNotFound()
        {
            var result = _friendsRepository.AcceptFriendRequest(1, 3);

            Assert.AreEqual("Friend request not found.", result.Error);
        }

        [TestMethod]
        public void Test_AcceptFriendRequest_ShouldHandleSqlException()
        {
            _mockContext.Setup(c => c.Friend).Throws(new InvalidOperationException("Database error"));

            var result = _friendsRepository.AcceptFriendRequest(1, 2);

            Assert.IsTrue(result.Error.Contains("Database error"));
        }

        [TestMethod]
        public void Test_DeclineFriendRequest_ShouldReturnSuccess_WhenRequestIsDeclined()
        {
            _fakeFriendSet.FirstOrDefault(f => f.PlayerId == 1 && f.FriendId == 2).Status = "sent";

            var result = _friendsRepository.DeclineFriendRequest(1, 2);

            Assert.AreEqual("Friend request declined successfully.", result.Value);
        }

        [TestMethod]
        public void Test_DeclineFriendRequest_ShouldReturnFailure_WhenRequestNotFound()
        {
            var result = _friendsRepository.DeclineFriendRequest(1, 3);

            Assert.AreEqual("Friend request not found.", result.Error);
        }

        [TestMethod]
        public void Test_DeclineFriendRequest_ShouldHandleSqlException()
        {
            _mockContext.Setup(c => c.Friend).Throws(new InvalidOperationException("Database error"));

            var result = _friendsRepository.DeclineFriendRequest(1, 2);

            Assert.IsTrue(result.Error.Contains("Database error"));
        }

        [TestMethod]
        public void Test_RemoveFriend_ShouldReturnSuccess_WhenFriendIsRemoved()
        {
            _fakeFriendSet.Add(new Friend { PlayerId = 1, FriendId = 2, Status = "accepted" });

            var result = _friendsRepository.RemoveFriend(2, 1);

            Assert.AreEqual("Friend removed successfully.", result.Value);
        }

        [TestMethod]
        public void Test_RemoveFriend_ShouldReturnFailure_WhenFriendshipNotFound()
        {
            var result = _friendsRepository.RemoveFriend(2, 3);

            Assert.AreEqual("Friendship not found.", result.Error);
        }

        [TestMethod]
        public void Test_RemoveFriend_ShouldHandleSqlException()
        {
            _mockContext.Setup(c => c.Friend).Throws(new InvalidOperationException("Database error"));

            var result = _friendsRepository.RemoveFriend(2, 1);

            Assert.IsTrue(result.Error.Contains("Database error"));
        }

        [TestMethod]
        public void Test_GetPendingFriendRequests_ShouldReturnRequests_WhenPendingRequestsExist()
        {
            _fakeFriendSet.Add(new Friend { PlayerId = 1, FriendId = 2, Status = "sent" });

            var result = _friendsRepository.GetPendingFriendRequests(2);

            Assert.AreEqual(2, result.Value.First().Id);
        }

        [TestMethod]
        public void Test_GetPendingFriendRequests_ShouldReturnFailure_WhenNoPendingRequestsExist()
        {
            var result = _friendsRepository.GetPendingFriendRequests(3);

            Assert.AreEqual("No pending friend requests found.", result.Error);
        }

        [TestMethod]
        public void Test_GetPendingFriendRequests_ShouldHandleSqlException()
        {
            _mockContext.Setup(c => c.Friend).Throws(new InvalidOperationException("Database error"));

            var result = _friendsRepository.GetPendingFriendRequests(2);

            Assert.IsTrue(result.Error.Contains("Database error"));
        }
    }
}
