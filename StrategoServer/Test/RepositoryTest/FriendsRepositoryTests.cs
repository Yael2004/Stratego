using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace StrategoDataAccess.Tests
{
    [TestClass]
    public class FriendsRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Mock<DbSet<Friend>> _mockFriendSet;
        private Mock<DbSet<Player>> _mockPlayerSet;
        private FriendsRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();
            _mockFriendSet = new Mock<DbSet<Friend>>();
            _mockPlayerSet = new Mock<DbSet<Player>>();

            _mockContext.Setup(c => c.Friend).Returns(_mockFriendSet.Object);
            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);

            _repository = new FriendsRepository();
        }

        [TestMethod]
        public void Test_SendFriendRequest_FriendRequestAlreadyExists_ShouldReturnFailure()
        {
            int requesterId = 1;
            int destinationId = 2;

            var existingRequest = new Friend
            {
                PlayerId = requesterId,
                FriendId = destinationId,
                Status = "sent"
            };

            var data = new List<Friend> { existingRequest }.AsQueryable();

            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var result = _repository.SendFriendRequest(destinationId, requesterId);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void Test_AcceptFriendRequest_RequestNotFound_ShouldReturnFailure()
        {
            int requesterId = 1;
            int destinationId = 2;

            var data = new List<Friend>().AsQueryable();

            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var result = _repository.AcceptFriendRequest(destinationId, requesterId);

            Assert.AreEqual("Friend request not found.", result.Error);
        }

        [TestMethod]
        public void Test_DeclineFriendRequest_RequestNotFound_ShouldReturnFailure()
        {
            int requesterId = 1;
            int destinationId = 2;

            var data = new List<Friend>().AsQueryable();

            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var result = _repository.DeclineFriendRequest(destinationId, requesterId);

            Assert.AreEqual("Friend request not found.", result.Error);
        }

        [TestMethod]
        public void Test_RemoveFriend_FriendshipNotFound_ShouldReturnFailure()
        {
            int requesterId = 1;
            int destinationId = 2;

            var data = new List<Friend>().AsQueryable();

            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var result = _repository.RemoveFriend(destinationId, requesterId);

            Assert.AreEqual("Friendship not found.", result.Error);
        }

        [TestMethod]
        public void Test_GetPendingFriendRequests_NoPendingRequests_ShouldReturnFailure()
        {
            int playerId = 1;

            var data = new List<Friend>().AsQueryable();
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockFriendSet.As<IQueryable<Friend>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var playerData = new List<Player>().AsQueryable();
            _mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.Provider).Returns(playerData.Provider);
            _mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.Expression).Returns(playerData.Expression);
            _mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.ElementType).Returns(playerData.ElementType);
            _mockPlayerSet.As<IQueryable<Player>>().Setup(m => m.GetEnumerator()).Returns(playerData.GetEnumerator());

            var result = _repository.GetPendingFriendRequests(playerId);

            Assert.AreEqual("No pending friend requests found.", result.Error);
        }
    }
}
