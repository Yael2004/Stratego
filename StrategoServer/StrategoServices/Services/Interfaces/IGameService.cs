﻿using StrategoServices.Data;
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
        Task<OperationResult> StartGameAsync(int player1Id, int player2Id);

        [OperationContract]
        Task<OperationResult> SendPositionAsync(int gameId, int playerId, PositionDTO position);

        [OperationContract]
        Task<OperationResult> EndGameAsync(int gameId, int playerId);

        [OperationContract]
        Task<OperationResult> AbandonGameAsync(int gameId, int playerId);
    }
}