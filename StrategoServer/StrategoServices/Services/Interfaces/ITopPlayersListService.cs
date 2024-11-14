using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(Callbacks.ITopPlayersListCallback))]
    public interface ITopPlayersListService
    {
        [OperationContract]
        Task GetTopPlayersListAsync();
    }
}
