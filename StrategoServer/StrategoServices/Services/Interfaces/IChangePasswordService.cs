using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(Interfaces.Callbacks.IChangePasswordServiceCallback))]
    public interface IChangePasswordService
    {
        [OperationContract]
        Task ObtainVerificationCodeAsync(string email);

        [OperationContract]
        Task SendVerificationCodeAsync(string email, string code);

        [OperationContract]
        Task SendNewPasswordAsync(string email, string newHashedPassword);
    }
}
