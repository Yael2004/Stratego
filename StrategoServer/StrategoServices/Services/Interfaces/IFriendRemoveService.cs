using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract (CallbackContract = typeof(IFriendRemoveServiceCallback))]
    public interface IFriendRemoveService
    {
        [OperationContract]
        Task RemoveFriend(int destinationId, int requesterId);
    }
}
