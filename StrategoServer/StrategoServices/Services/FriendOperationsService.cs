using StrategoServices.Data;
using StrategoServices.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FriendOperationsService : Interfaces.IFriendOperationsService, Interfaces.ISendRoomInvitationService
    {
        private readonly Lazy<FriendsManager> _friendsManager;
        private readonly Lazy<InvitationManager> _invitationManager;

        public FriendOperationsService(Lazy<FriendsManager> friendsManager, Lazy<InvitationManager> invitationManager)
        {
            _friendsManager = friendsManager;
            _invitationManager = invitationManager;
        }

        public async Task SendFriendRequest(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.SendFriendRequest(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException)
            {
                operationResult = new OperationResult(false, "Timeout error");
            }
            catch (Exception)
            {
                operationResult = new OperationResult(false, "Unexpected error");
            }

            await Task.Run(() => callback.GetFriendOperation(operationResult));
        }

        public async Task AcceptFriendRequest(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.AcceptFriendRequest(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException)
            {
                operationResult = new OperationResult(false, "Timeout error");
            }
            catch (Exception)
            {
                operationResult = new OperationResult(false, "Unexpected error");
            }

            await Task.Run(() => callback.GetFriendOperation(operationResult));
        }

        public async Task DeclineFriendRequest(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.DeclineFriendRequest(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException)
            {
                operationResult = new OperationResult(false, "Timeout error");
            }
            catch (Exception)
            {
                operationResult = new OperationResult(false, "Unexpected error");
            }

            await Task.Run(() => callback.GetFriendOperation(operationResult));
        }

        public async Task RemoveFriend(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.RemoveFriend(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException)
            {
                operationResult = new OperationResult(false, "Timeout error");
            }
            catch (Exception)
            {
                operationResult = new OperationResult(false, "Unexpected error");
            }

            await Task.Run(() => callback.GetFriendOperation(operationResult));
        }

        public async Task<bool> SendRoomInvitation(int playerId, string roomCode)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.ISendRoomInvitationServiceCallback>();
            OperationResult operationResult;
            bool response = false;

            var mailResult = _invitationManager.Value.GetPlayerMail(playerId);

            if (!mailResult.IsSuccess)
            {
                operationResult = new OperationResult(false, mailResult.Error);
            }
            else
            {
                EmailSender.Instance.SendEmail(mailResult.Value, roomCode);
                operationResult = new OperationResult(true, "Room invitation sent");
                response = true;
            }

            await Task.Run(() => callback.SendRoomInvitationResponseCall(operationResult));
            return response;
        }

    }
}
