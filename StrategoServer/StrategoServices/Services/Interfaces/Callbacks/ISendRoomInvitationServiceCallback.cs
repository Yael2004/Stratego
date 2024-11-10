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
    public interface ISendRoomInvitationServiceCallback
    {
        [OperationContract]
        void SendRoomInvitationResponseCall(OperationResult result);
    }
}
