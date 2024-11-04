using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract (CallbackContract = typeof(IFriendOperationCallback))]
    public interface IFriendOperationsService
    {
        [OperationContract]
        Task SendFriendRequest(int destinationId, int requesterId);

        [OperationContract]
        Task AcceptFriendRequest(int destinationId, int requesterId);

        [OperationContract]
        Task DeclineFriendRequest(int destinationId, int requesterId);

        [OperationContract]
        Task RemoveFriend(int destinationId, int requesterId);
    }
}
