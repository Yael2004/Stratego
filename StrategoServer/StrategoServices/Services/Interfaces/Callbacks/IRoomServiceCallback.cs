using StrategoServices.Data;
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
    public interface IRoomServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RoomCreatedAsync(RoomCreatedResponse response);

        [OperationContract(IsOneWay = true)]
        void RoomResponseAsync(OperationResult response);

        [OperationContract(IsOneWay = true)]
        void ReceiveMessageAsync(int playerId, string message);

        [OperationContract(IsOneWay = true)]
        void GetConnectedPlayerId(int connectedPlayerId);
    }
}
