using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(ISignUpServiceCallback))]
    public interface ISignUpService
    {
        [OperationContract]
        Task SignUpAsync(string email, string password, string playername);
    }
}
