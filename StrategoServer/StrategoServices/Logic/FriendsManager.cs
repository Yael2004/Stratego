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

        public async Task<Result<string>> SendFriendRequestAsync(int destinationId, int requesterId)
        {
            if (destinationId == requesterId)
            {
                return Result<string>.Failure("Cannot send a friend request to yourself.");
            }

            return await _friendsRepository.Value.SendFriendRequest(destinationId, requesterId);
        }

        public async Task<Result<string>> AcceptFriendRequestAsync(int destinationId, int requesterId)
        {
            return await _friendsRepository.Value.AcceptFriendRequest(destinationId, requesterId);
        }

        public async Task<Result<string>> DeclineFriendRequestAsync(int destinationId, int requesterId)
        {
            return await _friendsRepository.Value.DeclineFriendRequest(destinationId, requesterId);
        }

        public async Task<Result<string>> RemoveFriendAsync(int destinationId, int requesterId)
        {
            return await _friendsRepository.Value.RemoveFriend(destinationId, requesterId);
        }

    }
}
