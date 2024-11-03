using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract (CallbackContract = typeof(IOtherProfileDataCallback))]
    public interface IOtherProfileDataService
    {
        [OperationContract]
        Task GetOtherPlayerInfoAsync(int playerId, int requesterPlayerId);
    }
}
