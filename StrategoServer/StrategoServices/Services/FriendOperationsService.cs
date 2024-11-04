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
    public class FriendOperationsService : Interfaces.IFriendOperationsService
    {
        private readonly Lazy<FriendsManager> _friendsManager;

        public FriendOperationsService(Lazy<FriendsManager> friendsManager)
        {
            _friendsManager = friendsManager;
        }

        public async Task SendFriendRequest(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = await _friendsManager.Value.SendFriendRequestAsync(destinationId, requesterId);
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
                var result = await _friendsManager.Value.AcceptFriendRequestAsync(destinationId, requesterId);
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
                var result = await _friendsManager.Value.DeclineFriendRequestAsync(destinationId, requesterId);
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
                var result = await _friendsManager.Value.RemoveFriendAsync(destinationId, requesterId);
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

    }
}
