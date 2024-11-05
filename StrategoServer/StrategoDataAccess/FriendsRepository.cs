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
        private readonly Lazy<StrategoEntities> _context;

        public FriendsRepository(Lazy<StrategoEntities> context)
        {
            _context = context;
        }

        public Result<string> SendFriendRequest(int destinationId, int requesterId)
        {
            try
            {
                var existingRequest = _context.Value.Friend
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

                    _context.Value.Friend.Add(friendRequest);
                }

                _context.Value.SaveChanges();
                return Result<string>.Success("Friend request sent successfully.");
            }
            catch (SqlException sqlEx)
            {
                return Result<string>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public Result<string> AcceptFriendRequest(int destinationId, int requesterId)
        {
            try
            {
                var request = _context.Value.Friend
                    .FirstOrDefault(f => f.PlayerId == destinationId && f.FriendId == requesterId && f.Status == "sent");

                if (request == null)
                {
                    return Result<string>.Failure("Friend request not found.");
                }

                request.Status = "accepted";
                _context.Value.SaveChanges();

                return Result<string>.Success("Friend request accepted successfully.");
            }
            catch (SqlException sqlEx)
            {
                return Result<string>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public Result<string> DeclineFriendRequest(int destinationId, int requesterId)
        {
            try
            {
                var request = _context.Value.Friend
                    .FirstOrDefault(f => f.PlayerId == destinationId && f.FriendId == requesterId && f.Status == "sent");

                if (request == null)
                {
                    return Result<string>.Failure("Friend request not found.");
                }

                request.Status = "canceled";
                _context.Value.SaveChanges();

                return Result<string>.Success("Friend request declined successfully.");
            }
            catch (SqlException sqlEx)
            {
                return Result<string>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public Result<string> RemoveFriend(int destinationId, int requesterId)
        {
            try
            {
                var friendship = _context.Value.Friend
                    .FirstOrDefault(f => (f.PlayerId == requesterId && f.FriendId == destinationId && f.Status == "accepted") ||
                                              (f.PlayerId == destinationId && f.FriendId == requesterId && f.Status == "accepted"));

                if (friendship == null)
                {
                    return Result<string>.Failure("Friendship not found.");
                }

                friendship.Status = "canceled";
                _context.Value.SaveChanges();

                return Result<string>.Success("Friend removed successfully.");
            }
            catch (SqlException sqlEx)
            {
                return Result<string>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

    }
}
