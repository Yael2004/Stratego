using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Logic
{
    public class FriendsManager
    {
        private readonly Lazy<FriendsRepository> _friendsRepository;

        public FriendsManager(Lazy<FriendsRepository> friendsRepository)
        {
            _friendsRepository = friendsRepository;
        }

        public Result<string> SendFriendRequest(int destinationId, int requesterId)
        {
            if (destinationId == requesterId)
            {
                return Result<string>.Failure("Cannot send a friend request to yourself.");
            }

            return _friendsRepository.Value.SendFriendRequest(destinationId, requesterId);
        }

        public Result<string> AcceptFriendRequest(int destinationId, int requesterId)
        {
            return _friendsRepository.Value.AcceptFriendRequest(destinationId, requesterId);
        }

        public Result<string> DeclineFriendRequest(int destinationId, int requesterId)
        {
            return _friendsRepository.Value.DeclineFriendRequest(destinationId, requesterId);
        }

        public Result<string> RemoveFriend(int destinationId, int requesterId)
        {
            return _friendsRepository.Value.RemoveFriend(destinationId, requesterId);
        }

        public Result<IEnumerable<Player>> GetFriendRequestsFromRepository(int playerId)
        {
            return _friendsRepository.Value.GetPendingFriendRequests(playerId);
        }

        public Result<List<int>> GetFriendRequestIdsList(int playerId)
        {
            var friendRequestsResult = GetFriendRequestsFromRepository(playerId);

            if (!friendRequestsResult.IsSuccess)
            {
                return Result<List<int>>.Failure(friendRequestsResult.Error);
            }

            var friendRequestsIds = friendRequestsResult.Value.Select(f => f.Id).ToList();

            return friendRequestsIds.Any() 
                ? Result<List<int>>.Success(friendRequestsIds) 
                : Result<List<int>>.Failure("No friend requests found.");
        }

    }
}
