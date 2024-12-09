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
    [ServiceContract(CallbackContract = typeof(IProfileDataServiceCallback))]
    public interface IProfileDataService
    {
        [OperationContract]
        Task GetPlayerStatisticsAsync(int playerAccountId);

        [OperationContract]
        void LogOut(int playerId);
    }

}
