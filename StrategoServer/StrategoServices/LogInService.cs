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

        public void LogIn(string email, string password)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            var result = _accountManager.LogInAccount(email, password);

            if (result.IsSuccess)
            {
                callback.LogInResult(new OperationResult(true, "Login succesful"));
            }
            else
            {
                callback.LogInResult(new OperationResult(false, result.Error));
            }
        }

        public void SignUp(string email, string password, string playername)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            var result = _accountManager.CreateAccount(email, password, playername);

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