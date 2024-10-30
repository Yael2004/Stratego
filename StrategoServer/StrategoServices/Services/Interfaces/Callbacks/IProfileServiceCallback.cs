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
    public interface IProfileServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PlayerInfo(PlayerInfoResponse playerInfo);

        [OperationContract(IsOneWay = true)]
        void PlayerStatistics(PlayerStatisticsResponse playerStatistics);

        [OperationContract(IsOneWay = true)]
        void ReceiveUpdatePlayerProfile(PlayerInfoResponse result);

        [OperationContract(IsOneWay = true)]
        void PlayerFriendsList(PlayerFriendsResponse playerFriends);
    }
}
