using StrategoServices.Data.DTO;
using StrategoServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using StrategoServices.Services.Interfaces.Callbacks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IProfileServiceCallback))]
    public interface IProfileService
    {
        [OperationContract]
        Task GetPlayerInfoAsync(int playerId);

        [OperationContract]
        Task UpdatePlayerProfileAsync(PlayerInfoShownDTO newProfile);

        [OperationContract]
        Task GetPlayerStatisticsAsync(int playerId);
    }

}
