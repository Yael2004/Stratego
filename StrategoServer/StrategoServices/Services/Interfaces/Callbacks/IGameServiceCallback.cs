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
        void OnGameStarted(OperationResult result, int gameId);

        [OperationContract(IsOneWay = true)]
        void OnReceiveOpponentPosition(OperationResult result, PositionDTO position);

        [OperationContract(IsOneWay = true)]
        void OnOpponentAbandonedGame();

        [OperationContract(IsOneWay = true)]
        void OnGameEnded(OperationResult result, string advice);
    }
}
