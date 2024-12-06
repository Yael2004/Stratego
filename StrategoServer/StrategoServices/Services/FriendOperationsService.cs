using log4net;
using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FriendOperationsService : Interfaces.IFriendOperationsService, Interfaces.ISendRoomInvitationService, Interfaces.IPlayerFriendRequestService
    {
        private readonly Lazy<FriendsManager> _friendsManager;
        private readonly Lazy<InvitationManager> _invitationManager;
        private static readonly ILog log = LogManager.GetLogger(typeof(FriendOperationsService));

        public FriendOperationsService(Lazy<FriendsManager> friendsManager, Lazy<InvitationManager> invitationManager)
        {
            _friendsManager = friendsManager;
            _invitationManager = invitationManager;
        }

        /// <summary>
        /// Add a new friend request to the database associated to the requesterId and destinationId. Set pending status
        /// </summary>
        /// <param name="destinationId"></param>
        /// <param name="requesterId"></param>
        /// <returns>Task</returns>
        public async Task SendFriendRequest(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.SendFriendRequest(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                operationResult = new OperationResult(false, Messages.TimeoutError);
            }
            catch (CommunicationException cex)
            {
                log.Error(Messages.CommunicationError, cex);
                operationResult = new OperationResult(false, Messages.CommunicationError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                operationResult = new OperationResult(false, Messages.UnexpectedError);
            }

            await Task.Run(() => callback.GetFriendOperationSend(operationResult));
        }

        /// <summary>
        /// Accept a friend request from the requesterId to the destinationId. Set accepted status
        /// </summary>
        /// <param name="destinationId"></param>
        /// <param name="requesterId"></param>
        /// <returns></returns>
        public async Task AcceptFriendRequest(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.AcceptFriendRequest(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                operationResult = new OperationResult(false, Messages.TimeoutError);
            }
            catch (CommunicationException cex)
            {
                log.Error(Messages.CommunicationError, cex);
                operationResult = new OperationResult(false, Messages.CommunicationError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                operationResult = new OperationResult(false, Messages.UnexpectedError);
            }

            await Task.Run(() => callback.GetFriendOperationAccept(operationResult));
        }

        /// <summary>
        /// Decline a friend request from the requesterId to the destinationId. Set canceled status
        /// </summary>
        /// <param name="destinationId"></param>
        /// <param name="requesterId"></param>
        /// <returns></returns>
        public async Task DeclineFriendRequest(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.DeclineFriendRequest(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                operationResult = new OperationResult(false, Messages.TimeoutError);
            }
            catch (CommunicationException cex)
            {
                log.Error(Messages.CommunicationError, cex);
                operationResult = new OperationResult(false, Messages.CommunicationError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                operationResult = new OperationResult(false, Messages.UnexpectedError);
            }

            await Task.Run(() => callback.GetFriendOperationDecline(operationResult));
        }

        /// <summary>
        /// Remove a friend from the database associated to the requesterId and destinationId. Set canceled status
        /// </summary>
        /// <param name="destinationId"></param>
        /// <param name="requesterId"></param>
        /// <returns></returns>
        public async Task RemoveFriend(int destinationId, int requesterId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IFriendOperationCallback>();
            OperationResult operationResult;

            try
            {
                var result = _friendsManager.Value.RemoveFriend(destinationId, requesterId);
                operationResult = new OperationResult(result.IsSuccess, result.Error);
            }
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                operationResult = new OperationResult(false, Messages.TimeoutError);
            }
            catch (CommunicationException cex)
            {
                log.Error(Messages.CommunicationError, cex);
                operationResult = new OperationResult(false, Messages.CommunicationError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                operationResult = new OperationResult(false, Messages.UnexpectedError);
            }

            await Task.Run(() => callback.GetFriendOperationRemove(operationResult));
        }

        /// <summary>
        /// Sends an email to the player with the room code
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="roomCode"></param>
        /// <returns></returns>
        public async Task<bool> SendRoomInvitation(int playerId, string roomCode)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.ISendRoomInvitationServiceCallback>();
            OperationResult operationResult;
            bool response = false;

            try
            {
                var mailResult = _invitationManager.Value.GetPlayerMail(playerId);

                if (!mailResult.IsSuccess)
                {
                    operationResult = new OperationResult(false, mailResult.Error);
                }
                else
                {
                    var sendingResult = EmailSender.Instance.SendInvitationEmail(mailResult.Value, roomCode);
                    if (!sendingResult)
                    {
                        operationResult = new OperationResult(false, "Failed to send room invitation");
                    }
                    else
                    {
                        operationResult = new OperationResult(true, "Room invitation sent");
                        response = true;
                    }
                }
            }
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                operationResult = new OperationResult(false, Messages.TimeoutError);
            }
            catch (CommunicationException cex)
            {
                log.Error(Messages.CommunicationError, cex);
                operationResult = new OperationResult(false, Messages.CommunicationError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                operationResult = new OperationResult(false, Messages.UnexpectedError);
            }

            await Task.Run(() => callback.SendRoomInvitationResponseCall(operationResult));
            return response;
        }

        /// <summary>
        /// Obtains the list of friend requests for the player
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task GetPlayerFriendRequestAsync(int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IPlayerFriendRequestServiceCallback>();
            var response = new PlayerFriendRequestResponse();

            try
            {
                var result = _friendsManager.Value.GetFriendRequestIdsList(playerId);

                if (!result.IsSuccess)
                {
                    response.Result = new OperationResult(false, result.Error);
                }
                else
                {
                    response.FriendRequestIds = result.Value;
                    response.Result = new OperationResult(true, "Friend requests retrieved successfully");
                }
            }
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                response.Result = new OperationResult(false, Messages.TimeoutError);
            }
            catch (CommunicationException cex)
            {
                log.Error(Messages.CommunicationError, cex);
                response.Result = new OperationResult(false, Messages.CommunicationError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                response.Result = new OperationResult(false, Messages.UnexpectedError);
            }

            await Task.Run(() => callback.ReceiveFriendRequestIds(response));
        }

    }
}
