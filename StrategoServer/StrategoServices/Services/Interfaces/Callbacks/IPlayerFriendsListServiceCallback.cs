using StrategoServices.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces.Callbacks
{
    [ServiceContract]
    public interface IPlayerFriendsListServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PlayerFriendsList(PlayerFriendsResponse playerFriends);
    }
}
