using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using StrategoServices.Services.Interfaces;
using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LogInService : ILogInService, ISignUpService, IChangePasswordService
    {
        private readonly Lazy<AccountManager> _accountManager;
        private readonly Lazy<PasswordManager> _passwordManager;
        private readonly ConnectedPlayersManager _connectedPlayersManager;

        public LogInService(Lazy<AccountManager> accountManager, Lazy<PasswordManager> passwordManager, ConnectedPlayersManager connectedPlayersManager)
        {
            _accountManager = accountManager;
            _passwordManager = passwordManager;
            _connectedPlayersManager = connectedPlayersManager;
        }

        public async Task LogInAsync(string email, string password)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            var loginResult = _accountManager.Value.LogInAccount(email, password);
            if (!loginResult.IsSuccess)
            {
                await Task.Run(() => callback.LogInResult(new OperationResult(false, loginResult.Error)));
                return;
            }

            var playerId = loginResult.Value;
            if (_connectedPlayersManager.IsPlayerConnected(playerId))
            {
                await Task.Run(() => callback.LogInResult(new OperationResult(false, "User is already logged in.")));
                return;
            }

            var playerResult = _accountManager.Value.GetLogInAccount(playerId);
            if (!playerResult.IsSuccess)
            {
                await Task.Run(() => callback.LogInResult(new OperationResult(false, playerResult.Error)));
                return;
            }

            var playerInfo = playerResult.Value;
            var playerAdded = _connectedPlayersManager.AddPlayer(playerInfo.Id, playerInfo.Name);
            if (!playerAdded)
            {
                await Task.Run(() => callback.LogInResult(new OperationResult(false, "Failed to add player to connected players list.")));
                return;
            }

            await Task.Run(() =>
            {
                callback.AccountInfo(playerInfo);
                callback.LogInResult(new OperationResult(true, "Login successful"));
            });
        }

        public async Task SignUpAsync(string email, string password, string playername)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ISignUpServiceCallback>();

            var result = _accountManager.Value.CreateAccount(email, password, playername);

            if (result.IsSuccess)
            {
                await Task.Run(() => callback.SignUpResult(new OperationResult(true, "Account created successfully")));
            }
            else
            {
                await Task.Run(() => callback.SignUpResult(new OperationResult(false, result.Error)));
            }
        }

        public async Task<bool> ObtainVerificationCodeAsync(string email)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChangePasswordServiceCallback>();
            OperationResult response;
            bool isSuccessResponse = false;

            var accountExistsResult = _passwordManager.Value.AlreadyExistentAccount(email);
            if (!accountExistsResult.IsSuccess || !accountExistsResult.Value)
            {
                response = new OperationResult(false, "Account not found");
            } 
            else
            {
                var verificationCode = _passwordManager.Value.GenerateVerificationCode(email);
                EmailSender.Instance.SendVerificationEmail(email, verificationCode);
                response = new OperationResult(true, "Verification code sent.");
                isSuccessResponse = true;
            }

            await Task.Run(() => callback.ChangePasswordResult(response));
            return isSuccessResponse;
        }

        public async Task<bool> SendVerificationCodeAsync(string email, string code)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChangePasswordServiceCallback>();
            OperationResult response;
            bool isValid = false;

            var verificationResult = _passwordManager.Value.ValidateVerificationCode(email, code);
            if (!verificationResult.IsSuccess)
            {
                response = new OperationResult(false, "Invalid verification code");
            }
            else
            {
                response = new OperationResult(true, "Verification code is correct");
                isValid = true;
            }

            await Task.Run(() => callback.ChangePasswordResult(response));
            return isValid;
        }

        public async Task SendNewPasswordAsync(string email, string newHashedPassword)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChangePasswordServiceCallback>();

            var result = _passwordManager.Value.ChangePassword(email, newHashedPassword);
            OperationResult response;

            if (!result.IsSuccess)
            {
                response = new OperationResult(false, result.Error);
            }
            else
            {
                response = new OperationResult(true, "Password changed successfully");
            }

            await Task.Run(() => callback.ChangePasswordResult(response));
        }

    }
}