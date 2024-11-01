using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IRoomServiceCallback))]
    public interface IRoomService
    {
        [OperationContract]
        Task CreateRoomAsync(string playerId);

        [OperationContract]
        Task JoinRoomAsync(string roomCode, string playerId);

        [OperationContract(IsOneWay = true)]
        void LeaveRoomAsync(string playerId);

        [OperationContract]
        Task SendMessageToRoomAsync(string roomCode, string playerId, string message);
    }
}
