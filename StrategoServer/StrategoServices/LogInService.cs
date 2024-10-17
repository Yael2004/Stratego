using StrategoCore.Services;
using StrategoServices.Data;
using StrategoServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LogInService : ILogInService
    {
        private readonly AccountManager _accountManager;

        public LogInService(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task LogIn(string email, string password)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            var result = await _accountManager.LogInAccountAsync(email, password);

            if (result.IsSuccess)
            {
                callback.SignUpResult(new OperationResult(true, "Login succesful"));
            }
            else
            {
                callback.SignUpResult(new OperationResult(false, result.Error));
            }
        }

        public async Task SignUp(string email, string password)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            var result = await _accountManager.CreateAccountAsync(email, password);

            if (result.IsSuccess)
            {
                callback.SignUpResult(new OperationResult(true, "Account created successfully"));
            }
            else
            {
                callback.SignUpResult(new OperationResult(false, result.Error));
            }
        }
    }
}