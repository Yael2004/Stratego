using StrategoServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces.Callbacks
{
    [ServiceContract]
    public interface IFriendOperationCallback
    {
        [OperationContract]
        void GetFriendOperationSend(OperationResult result);

        [OperationContract]
        void GetFriendOperationAccept(OperationResult result);

        [OperationContract]
        void GetFriendOperationDecline(OperationResult result);

        [OperationContract]
        void GetFriendOperationRemove(OperationResult result);
    }
}
