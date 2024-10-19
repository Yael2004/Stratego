using StrategoServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract]
    public interface ISignUpServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void SignUpResult(OperationResult result);
    }
}
