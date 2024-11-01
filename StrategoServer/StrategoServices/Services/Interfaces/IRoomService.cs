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
        Task CreateRoomAsync(int playerId);

        [OperationContract]
        Task JoinRoomAsync(string roomCode, int playerId);

        [OperationContract(IsOneWay = true)]
        void LeaveRoomAsync(int playerId);

        [OperationContract]
        Task SendMessageToRoomAsync(string roomCode, int playerId, string message);
    }
}
