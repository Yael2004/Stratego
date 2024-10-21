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

        public async Task LogIn(string email, string password)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            var result = await _accountManager.LogInAccountAsync(email, password);

            if (result.IsSuccess)
            {
                var resultPlayer = await _accountManager.GetLogInAccountAsync(result.Value);

                if (resultPlayer.IsSuccess)
                {
                    await callback.AccountInfo(new PlayerDTO
                    {
                        Id = resultPlayer.Value.Id,
                        Name = resultPlayer.Value.Name,
                        AccountId = resultPlayer.Value.AccountId ?? 0
                    });

                    await callback.LogInResult(new OperationResult(true, "Login successful"));
                }
                else
                {
                    await callback.LogInResult(new OperationResult(false, resultPlayer.Error));
                }
            }
            else
            {
                await callback.LogInResult(new OperationResult(false, result.Error));
            }
        }

        public async Task SignUp(string email, string password, string playername)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ISignUpServiceCallback>();

            var result = await _accountManager.CreateAccountAsync(email, password, playername);

            if (result.IsSuccess)
            {
                await callback.SignUpResult(new OperationResult(true, "Account created successfully"));
            }
            else
            {
                await callback.SignUpResult(new OperationResult(false, result.Error));
            }
        }
    }
}