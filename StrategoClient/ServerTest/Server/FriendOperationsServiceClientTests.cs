using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.FriendService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest.Server
{
    [TestClass]
    public class FriendOperationsServiceClientTests : IFriendOperationsServiceCallback, ISendRoomInvitationServiceCallback, IPlayerFriendRequestServiceCallback, IFriendRemoveServiceCallback
    {
        private FriendOperationsServiceClient _friendOperationsClient;
        private SendRoomInvitationServiceClient _sendRoomInvitationServiceClient;
        private PlayerFriendRequestServiceClient _playerFriendRequestServiceClient;
        private FriendRemoveServiceClient _friendRemoveServiceClient;

        private OperationResult _sendRoomInvitationResponse;
        private PlayerFriendRequestResponse _playerFriendRequestResponse;
        private OperationResult _friendOperationAccept;
        private OperationResult _friendOperationDecline;
        private OperationResult _friendOperationRemove;
        private OperationResult _friendOperationSend;

        [TestInitialize]
        public void Initialize()
        {
            _friendOperationsClient = new FriendOperationsServiceClient(new InstanceContext(this));
            _sendRoomInvitationServiceClient = new SendRoomInvitationServiceClient(new InstanceContext(this));
            _playerFriendRequestServiceClient = new PlayerFriendRequestServiceClient(new InstanceContext(this));
            _friendRemoveServiceClient = new FriendRemoveServiceClient(new InstanceContext(this));
        }

        [TestMethod]
        public async Task Test_SendFriendRequest_Success()
        {
            await _friendOperationsClient.SendFriendRequestAsync(6, 7);
            await Task.Delay(3000);
            Assert.IsTrue(_friendOperationSend.IsSuccess);
        }

        [TestMethod]
        public async Task Test_AcceptFriendRequest_Success()
        {
            await _friendOperationsClient.AcceptFriendRequestAsync(6, 7);
            await Task.Delay(3000);
            Assert.IsTrue(_friendOperationAccept.IsSuccess);
        }

        [TestMethod]
        public async Task Test_DeclineFriendRequest_Success()
        {
            await _friendOperationsClient.DeclineFriendRequestAsync(6, 7);
            await Task.Delay(3000);
            Assert.IsTrue(_friendOperationDecline.IsSuccess);
        }

        [TestMethod]
        public async Task Test_SendRoomInvitation_Success()
        {
            await _sendRoomInvitationServiceClient.SendRoomInvitationAsync(6, "12345");
            await Task.Delay(3000);
            Assert.IsTrue(_sendRoomInvitationResponse.IsSuccess);
        }

        [TestMethod]
        public async Task Test_GetFriendRequestIds_Success()
        {
            await _playerFriendRequestServiceClient.GetPlayerFriendRequestAsync(6);
            await Task.Delay(3000);
            Assert.IsTrue(_playerFriendRequestResponse.Result.IsSuccess);
        }

        public void ReceiveFriendRequestIds(PlayerFriendRequestResponse response)
        {
            _playerFriendRequestResponse = response;
        }

        public void SendRoomInvitationResponseCall(OperationResult result)
        {
            _sendRoomInvitationResponse = result;
        }

        public void GetFriendOperationAccept(OperationResult result)
        {
            _friendOperationAccept = result;
        }

        public void GetFriendOperationDecline(OperationResult result)
        {
            _friendOperationDecline = result;
        }

        public void GetFriendOperationRemove(OperationResult result)
        {
            _friendOperationRemove = result;
        }

        public void GetFriendOperationSend(OperationResult result)
        {
            _friendOperationSend = result;
        }
    }
}
