using StrategoServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(ILogInServiceCallback))]
    public interface ILogInService
    {
        [OperationContract]
        Task LogInAsync(string email, string password);
    }
}
