using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(LogInService));

        public LogInService(Lazy<AccountManager> accountManager, Lazy<PasswordManager> passwordManager, ConnectedPlayersManager connectedPlayersManager)
        {
            _accountManager = accountManager;
            _passwordManager = passwordManager;
            _connectedPlayersManager = connectedPlayersManager;
        }

        /// <summary>
        /// Logs in the user with the given email and password if account exists and is not already logged in or has too many reports.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Task</returns>
        public async Task LogInAsync(string email, string password)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILogInServiceCallback>();

            try
            {
                var loginResult = _accountManager.Value.LogInAccount(email, password);
                if (!loginResult.IsSuccess)
                {
                    await NotifyCallbackAsync(callback.LogInResult, new OperationResult(false, loginResult.Error));
                    return;
                }

                var playerId = loginResult.Value;
                if (_connectedPlayersManager.IsPlayerConnected(playerId))
                {
                    await NotifyCallbackAsync(callback.LogInResult, new OperationResult(false, "User is already logged in."));
                    return;
                }

                var playerResult = _accountManager.Value.GetLogInAccount(playerId);
                if (!playerResult.IsSuccess)
                {
                    await NotifyCallbackAsync(callback.LogInResult, new OperationResult(false, playerResult.Error));
                    return;
                }

                var playerInfo = playerResult.Value;
                var playerAdded = _connectedPlayersManager.AddPlayer(playerInfo.Id, playerInfo.Name);
                if (!playerAdded)
                {
                    await NotifyCallbackAsync(callback.LogInResult, new OperationResult(false, "Failed to add player to connected players list."));
                    return;
                }

                await NotifyCallbackAsync(() =>
                {
                    callback.AccountInfo(playerInfo);
                    callback.LogInResult(new OperationResult(true, "Login successful"));
                });
            }
            catch (TimeoutException tex)
            {
                log.Error("TimeoutException during LogInAsync", tex);
                await NotifyCallbackAsync(callback.LogInResult, new OperationResult(false, "The operation timed out."));
            }
            catch (CommunicationException cex)
            {
                log.Error("CommunicationException during LogInAsync", cex);
                await NotifyCallbackAsync(callback.LogInResult, new OperationResult(false, "A communication error occurred."));
            }
            catch (Exception ex)
            {
                log.Error("Exception during LogInAsync", ex);
                await NotifyCallbackAsync(callback.LogInResult, new OperationResult(false, "An error occurred during login."));
            }
        }

        /// <summary>
        /// Creates a new account with the given email, password and playername.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="playername"></param>
        /// <returns>Task</returns>
        public async Task SignUpAsync(string email, string password, string playername)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ISignUpServiceCallback>();

            try
            {
                var result = _accountManager.Value.CreateAccount(email, password, playername);

                await NotifyCallbackAsync(callback.SignUpResult, result.IsSuccess
                    ? new OperationResult(true, "Account created successfully")
                    : new OperationResult(false, result.Error));
            }
            catch (TimeoutException tex)
            {
                log.Error("TimeoutException during SignUpAsync", tex);
                await NotifyCallbackAsync(callback.SignUpResult, new OperationResult(false, "The operation timed out."));
            }
            catch (CommunicationException cex)
            {
                log.Error("CommunicationException during SignUpAsync", cex);
                await NotifyCallbackAsync(callback.SignUpResult, new OperationResult(false, "A communication error occurred."));
            }
            catch (Exception ex)
            {
                log.Error("Exception during SignUpAsync", ex);
                await NotifyCallbackAsync(callback.SignUpResult, new OperationResult(false, "An error occurred during sign up."));
            }
        }

        /// <summary>
        /// Sends a verification code to the given emailfor changing password.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Task</returns>
        public async Task<bool> ObtainVerificationCodeAsync(string email)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChangePasswordServiceCallback>();
            OperationResult response;
            bool isSuccessResponse = false;

            try
            {
                var accountExistsResult = _passwordManager.Value.AlreadyExistentAccount(email);
                if (!accountExistsResult.IsSuccess || !accountExistsResult.Value)
                {
                    response = new OperationResult(false, "Account not found");
                }
                else
                {
                    var verificationCode = _passwordManager.Value.GenerateVerificationCode(email);
                    var sendingResult = EmailSender.Instance.SendVerificationEmail(email, verificationCode);
                    if (!sendingResult)
                    {
                        response = new OperationResult(false, "Failed to send verification code");
                    }
                    else
                    {
                        response = new OperationResult(true, "Verification code sent.");
                        isSuccessResponse = true;
                    }
                }

                await NotifyCallbackAsync(callback.ChangePasswordResult, response);
            }
            catch (TimeoutException tex)
            {
                log.Error("TimeoutException during ObtainVerificationCodeAsync", tex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "The operation timed out."));
            }
            catch (CommunicationException cex)
            {
                log.Error("CommunicationException during ObtainVerificationCodeAsync", cex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "A communication error occurred."));
            }
            catch (Exception ex)
            {
                log.Error("Exception during ObtainVerificationCodeAsync", ex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "An error occurred while obtaining the verification code."));
            }

            return isSuccessResponse;
        }

        /// <summary>
        /// Validates the verification code sended to the given email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns>Task</returns>
        public async Task<bool> SendVerificationCodeAsync(string email, string code)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChangePasswordServiceCallback>();
            OperationResult response;
            bool isValid = false;

            try
            {
                var verificationResult = _passwordManager.Value.ValidateVerificationCode(email, code);
                response = verificationResult.IsSuccess
                    ? new OperationResult(true, "Verification code is correct")
                    : new OperationResult(false, "Invalid verification code");

                isValid = verificationResult.IsSuccess;

                await NotifyCallbackAsync(callback.ChangePasswordResult, response);
            }
            catch (TimeoutException tex)
            {
                log.Error("TimeoutException during SendVerificationCodeAsync", tex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "The operation timed out."));
            }
            catch (CommunicationException cex)
            {
                log.Error("CommunicationException during SendVerificationCodeAsync", cex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "A communication error occurred."));
            }
            catch (Exception ex)
            {
                log.Error("Exception during SendVerificationCodeAsync", ex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "An error occurred while sending the verification code."));
            }

            return isValid;
        }

        /// <summary>
        /// Updates the password of the account with the given email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newHashedPassword"></param>
        /// <returns>Task</returns>
        public async Task SendNewPasswordAsync(string email, string newHashedPassword)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChangePasswordServiceCallback>();

            try
            {
                var result = _passwordManager.Value.ChangePassword(email, newHashedPassword);
                var response = result.IsSuccess
                    ? new OperationResult(true, "Password changed successfully")
                    : new OperationResult(false, result.Error);

                await NotifyCallbackAsync(callback.ChangePasswordResult, response);
            }
            catch (TimeoutException tex)
            {
                log.Error("TimeoutException during SendNewPasswordAsync", tex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "The operation timed out."));
            }
            catch (CommunicationException cex)
            {
                log.Error("CommunicationException during SendNewPasswordAsync", cex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "A communication error occurred."));
            }
            catch (Exception ex)
            {
                log.Error("Exception during SendNewPasswordAsync", ex);
                await NotifyCallbackAsync(callback.ChangePasswordResult, new OperationResult(false, "An error occurred while changing the password."));
            }
        }

        /// <summary>
        /// Helper method to notify the client with the given result.
        /// </summary>
        /// <param name="callbackAction"></param>
        /// <param name="result"></param>
        /// <returns>Task</returns>
        private async Task NotifyCallbackAsync(Action<OperationResult> callbackAction, OperationResult result)
        {
            await Task.Run(() => callbackAction(result));
        }

        /// <summary>
        /// Helper method to notify the client with the given action.
        /// </summary>
        /// <param name="callbackAction"></param>
        /// <returns>Task</returns>
        private async Task NotifyCallbackAsync(Action callbackAction)
        {
            await Task.Run(callbackAction);
        }
    }
}