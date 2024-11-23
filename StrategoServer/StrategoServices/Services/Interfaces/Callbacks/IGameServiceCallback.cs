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
    public interface IGameServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnGameStarted(int gameId, OperationResult operationResult);

        [OperationContract(IsOneWay = true)]
        void OnReceiveOpponentPosition(PositionDTO position, OperationResult operationResult);

        [OperationContract(IsOneWay = true)]
        void OnOpponentAbandonedGame(OperationResult operationResult);

        [OperationContract(IsOneWay = true)]
        void OnGameEnded(string resultString, OperationResult operationResult);

        [OperationContract(IsOneWay = true)]
        void OnReceiveMovementInstructions(MovementInstructionResponse movementInstructionResponse);
    }
}
