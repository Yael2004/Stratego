using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using StrategoServices.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LogInService : ILogInService, ISignUpService
    {
        private readonly AccountManager _accountManager;

        public LogInService(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task LogInAsync(string email, string password)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            var loginResult = await _accountManager.LogInAccountAsync(email, password);

            if (!loginResult.IsSuccess)
            {
                callback.LogInResult(new OperationResult(false, loginResult.Error));
                return;
            }

            var playerResult = await _accountManager.GetLogInAccountAsync(loginResult.Value);

            if (!playerResult.IsSuccess)
            {
                callback.LogInResult(new OperationResult(false, playerResult.Error));
                return;
            }

            callback.AccountInfo(playerResult.Value);

            callback.LogInResult(new OperationResult(true, "Login successful"));
        }


        public async Task SignUpAsync(string email, string password, string playername)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ISignUpServiceCallback>();

            var result = await _accountManager.CreateAccountAsync(email, password, playername);

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