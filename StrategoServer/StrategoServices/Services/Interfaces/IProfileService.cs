using StrategoServices.Data.DTO;
using StrategoServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract]
    public interface IProfileService
    {
        [OperationContract]
        Task<PlayerInfoResponse> GetPlayerInfoAsync(int playerId);

        [OperationContract]
        Task<OperationResult> UpdatePlayerProfileAsync(PlayerInfoShownDTO profile);

        [OperationContract]
        Task<PlayerStatisticsResponse> GetPlayerStatisticsAsync(int playerId);
    }


}
