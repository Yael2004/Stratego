using StrategoServices.Data;
using StrategoServices.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract]
    public interface ILogInServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void LogInResult(OperationResult result);

        [OperationContract(IsOneWay = true)]
        void AccountInfo(PlayerDTO player);
    }
}
