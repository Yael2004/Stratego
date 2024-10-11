using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices
{
    [ServiceContract(CallbackContract = typeof(ILogInServiceCallback))]
    public interface ILogInService
    {
        [OperationContract(IsOneWay = true)]
        void LogIn(string username, string password);
    }

    [ServiceContract]
    public interface ILogInServiceCallback
    {
        [OperationContract]
        void LogInResult(bool success, string message);
    }
}
