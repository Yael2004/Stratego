using StrategoServices.Data;
using StrategoServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Interfaces
{
    [ServiceContract(CallbackContract = typeof(ILogInServiceCallback))]
    public interface ILogInService
    {
        [OperationContract]
        Task SignUp(string email, string password, string playername);

        [OperationContract]
        Task LogIn(string email, string password);
    }
}
