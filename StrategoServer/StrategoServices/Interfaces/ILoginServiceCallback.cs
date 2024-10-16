using StrategoServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Interfaces
{
    [ServiceContract]
    public interface ILogInServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void SignUpResult(OperationResult result);

        [OperationContract(IsOneWay = true)]
        void LogInResult(OperationResult result);
    }
}
