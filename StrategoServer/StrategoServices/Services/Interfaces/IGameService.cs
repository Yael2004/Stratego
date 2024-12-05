using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IGameServiceCallback))]
    public interface IGameService
    {
        [OperationContract]
        Task JoinGameSessionAsync(int gameId, int player2Id);

        [OperationContract]
        Task SendPositionAsync(int gameId, int playerId, PositionDTO position);

        [OperationContract]
        Task EndGameAsync(FinalStatsDTO finalStats);

        [OperationContract]
        Task AbandonGameAsync(int gameId, int playerId);

        [OperationContract]
        Task SendMovementInstructionsAsync(int gameId, MovementInstructionDTO movementInstruction);
    }
}
