using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoDataAccess
{
    public class FriendsRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FriendsRepository));

        public virtual Result<string> SendFriendRequest(int destinationId, int requesterId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var existingRequest = context.Friend
                    .FirstOrDefault(f => (f.PlayerId == requesterId && f.FriendId == destinationId) ||
                                              (f.PlayerId == destinationId && f.FriendId == requesterId));

                    if (existingRequest != null)
                    {
                        if (existingRequest.Status != "canceled")
                        {
                            return Result<string>.Failure("Friend request already exists or players are already friends.");
                        }

                        existingRequest.Status = "sent";
                    }
                    else
                    {
                        var friendRequest = new Friend
                        {
                            PlayerId = requesterId,
                            FriendId = destinationId,
                            Status = "sent"
                        };

                        context.Friend.Add(friendRequest);
                    }

                    context.SaveChanges();
                    return Result<string>.Success("Friend request sent successfully.");
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError}   : {ex.Message}");
            }
        }

        public virtual Result<string> AcceptFriendRequest(int destinationId, int requesterId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var request = context.Friend
                    .FirstOrDefault(f => f.PlayerId == destinationId && f.FriendId == requesterId && f.Status == "sent");

                    if (request == null)
                    {
                        return Result<string>.Failure("Friend request not found.");
                    }

                    request.Status = "accepted";
                    context.SaveChanges();

                    return Result<string>.Success("Friend request accepted successfully.");
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError}   : {ex.Message}");
            }
        }

        public virtual Result<string> DeclineFriendRequest(int destinationId, int requesterId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var request = context.Friend
                        .FirstOrDefault(f => f.PlayerId == destinationId && f.FriendId == requesterId && f.Status == "sent");

                    if (request == null)
                    {
                        return Result<string>.Failure("Friend request not found.");
                    }

                    request.Status = "canceled";
                    context.SaveChanges();

                    return Result<string>.Success("Friend request declined successfully.");
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError}  : {ex.Message}");
            }
        }

        public virtual Result<string> RemoveFriend(int destinationId, int requesterId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var friendship = context.Friend
                    .FirstOrDefault(f => (f.PlayerId == requesterId && f.FriendId == destinationId && f.Status == "accepted") ||
                                              (f.PlayerId == destinationId && f.FriendId == requesterId && f.Status == "accepted"));

                    if (friendship == null)
                    {
                        return Result<string>.Failure("Friendship not found.");
                    }

                    friendship.Status = "canceled";
                    context.SaveChanges();

                    return Result<string>.Success("Friend removed successfully.");
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError}  : {ex.Message}");
            }
        }

        public virtual Result<IEnumerable<Player>> GetPendingFriendRequests(int playerId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var pendingRequests = context.Friend
                    .Where(f => f.FriendId == playerId && f.Status == "sent")
                    .Join
                    (
                        context.Player,
                        friend => friend.PlayerId,
                        player => player.Id,
                        (friend, player) => player
                    )
                    .ToList();

                    if (!pendingRequests.Any())
                    {
                        return Result<IEnumerable<Player>>.Failure("No pending friend requests found.");
                    }

                    return Result<IEnumerable<Player>>.Success(pendingRequests);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<IEnumerable<Player>>.Failure($"{Messages.DataBaseError} : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<IEnumerable<Player>>.Failure($"{Messages.UnexpectedError}  : {ex.Message}");
            }
        }

    }
}
