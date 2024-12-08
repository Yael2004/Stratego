using log4net;
using StrategoApp.Helpers;
using StrategoApp.LogInService;
using StrategoApp.Model;
using StrategoApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Converters;

namespace StrategoApp.ViewModel
{
    public class LogInViewModel : ViewModelBase, LogInService.ILogInServiceCallback, LogInService.IChangePasswordServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _mail;
        private string _recoveryMail;
        private string _password;
        private string _editedPassword;
        private string _errorMessage;
        private string _exceptionMessage;
        private string _codeErrorMessage;
        private string _emailErrorMessage;
        private string _passwordErrorMessage;
        private string _passwordChangedErrorMessage;
        private string _togglePasswordVisibilityIcon;
        private string _codePart1;
        private string _codePart2;
        private string _codePart3;
        private string _codePart4;
        private string _codePart5;
        private string _codePart6;
        private bool _isServiceErrorVisible;
        private bool _isPasswordVisible;
        private bool _isPasswordChangedVisible;
        private bool _isDatabaseError;
        private bool _isForgotPasswordVisible;
        private bool _isCodeVerificationVisible;
        private bool _isChangePasswordVisible;
        private bool _passwordChangedSuccesfully;

        private readonly LogInServiceClient _logInServiceClient;
        private readonly ChangePasswordServiceClient _changePasswordServiceClient;

        private readonly MainWindowViewModel _mainWindowViewModel;

        public event Action ClearPasswordBox;

        public ICommand LogInCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand LogInAsInvitedCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand SendMailCommand { get; }
        public ICommand CancelSendMailCommand { get; }
        public ICommand VerifyCodeCommand { get; }
        public ICommand CancelVerificationCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand CancelChangePasswordCommand { get; }
        public ICommand CloseChangedPasswordMessageCommand { get; }

        public string CodePart1
        {
            get => _codePart1;
            set { _codePart1 = value; OnPropertyChanged(nameof(CodePart1)); }
        }
        public string CodePart2
        {
            get => _codePart2;
            set { _codePart2 = value; OnPropertyChanged(nameof(CodePart2)); }
        }
        public string CodePart3
        {
            get => _codePart3;
            set { _codePart3 = value; OnPropertyChanged(nameof(CodePart3)); }
        }
        public string CodePart4
        {
            get => _codePart4;
            set { _codePart4 = value; OnPropertyChanged(nameof(CodePart4)); }
        }
        public string CodePart5
        {
            get => _codePart5;
            set { _codePart5 = value; OnPropertyChanged(nameof(CodePart5)); }
        }
        public string CodePart6
        {
            get => _codePart6;
            set { _codePart6 = value; OnPropertyChanged(nameof(CodePart6)); }
        }

        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged();
            }
        }

        public string RecoveryMail
        {
            get { return _recoveryMail; }
            set
            {
                _recoveryMail = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string EditedPassword
        {
            get { return _editedPassword; }
            set
            {
                _editedPassword = value;
                OnPropertyChanged();
            }
        }
        public bool IsServiceErrorVisible
        {
            get { return _isServiceErrorVisible; }
            set
            {
                _isServiceErrorVisible = value;
                OnPropertyChanged();
            }
        }

        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set
            {
                _exceptionMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged(nameof(IsPasswordVisible));
            }
        }

        public bool IsPasswordChangedVisible
        {
            get => _isPasswordChangedVisible;
            set
            {
                _isPasswordChangedVisible = value;
                OnPropertyChanged(nameof(IsPasswordChangedVisible));
            }
        }

        public bool IsChangePasswordVisible
        {
            get { return _isChangePasswordVisible; }
            set
            {
                _isChangePasswordVisible = value;
                OnPropertyChanged();
            }
        }

        public string TogglePasswordVisibilityIcon
        {
            get { return _togglePasswordVisibilityIcon; }
            set
            {
                _togglePasswordVisibilityIcon = value;
                OnPropertyChanged();
            }
        }

        public bool IsForgotPasswordVisible
        {
            get { return _isForgotPasswordVisible; }
            set
            {
                _isForgotPasswordVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsCodeVerificationVisible
        {
            get { return _isCodeVerificationVisible; }
            set
            {
                _isCodeVerificationVisible = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string CodeErrorMessage
        {
            get { return _codeErrorMessage; }
            set
            {
                _codeErrorMessage = value;
                OnPropertyChanged(nameof(CodeErrorMessage));
            }
        }

        public string EmailErrorMessage
        {
            get { return _emailErrorMessage; }
            set
            {
                _emailErrorMessage = value;
                OnPropertyChanged(nameof(EmailErrorMessage));
            }
        }

        public string PasswordErrorMessage
        {
            get { return _passwordErrorMessage; }
            set
            {
                _passwordErrorMessage = value;
                OnPropertyChanged(nameof(PasswordErrorMessage));
            }
        }

        public bool PasswordChangedSuccesfully
        {
            get { return _passwordChangedSuccesfully; }
            set
            {
                _passwordChangedSuccesfully = value;
                OnPropertyChanged(nameof(PasswordChangedSuccesfully));
            }
        }

        public string PasswordChangedErrorMessage
        {
            get { return _passwordChangedErrorMessage; }
            set
            {
                _passwordChangedErrorMessage = value;
                OnPropertyChanged(nameof(PasswordChangedErrorMessage));
            }
        }

        public LogInViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _logInServiceClient = new LogInServiceClient(new InstanceContext(this));
            _changePasswordServiceClient = new ChangePasswordServiceClient(new InstanceContext(this));

            _mainWindowViewModel = mainWindowViewModel;

            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            LogInAsInvitedCommand = new ViewModelCommand(ExcuteLogInAsInvitedCommand);
            SignUpCommand = new ViewModelCommand(ExecuteSignUpCommand);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(ExecuteCloseServerError);
            ForgotPasswordCommand = new ViewModelCommand(ForgotPassword);
            CancelSendMailCommand = new ViewModelCommand(CancelForgotPassword);
            SendMailCommand = new ViewModelCommand(SendMail);
            VerifyCodeCommand = new ViewModelCommand(VerifyCode);
            CancelVerificationCommand = new ViewModelCommand(CancelVerification);
            ChangePasswordCommand = new ViewModelCommand(ChangePassword);
            CancelChangePasswordCommand = new ViewModelCommand(CancelChangePassword);
            CloseChangedPasswordMessageCommand = new ViewModelCommand(CloseChangedPasswordMessage);

            IsForgotPasswordVisible = false;
            IsServiceErrorVisible = false;
            IsPasswordVisible = false;
            IsPasswordChangedVisible = false;
            IsCodeVerificationVisible = false;
            IsChangePasswordVisible = false;
        }

        private void ForgotPassword(object obj)
        {
            IsForgotPasswordVisible = true;
        }

        private void CancelForgotPassword(object obj)
        {
            EmailErrorMessage = string.Empty;
            RecoveryMail = string.Empty;
            IsForgotPasswordVisible = false;
        }

        private void ChangePassword(object obj)
        {
            ChangePasswordClient();
        }

        private void CancelChangePassword(object obj)
        {
            IsChangePasswordVisible = false;

            EmptyFields();
        }

        private void SendMail(object obj)
        {
            EmailErrorMessage = string.Empty;

            ObtainVerificationCodeClient();
        }

        private async void VerifyCode(object obj)
        {
            string fullCode = $"{CodePart1}{CodePart2}{CodePart3}{CodePart4}{CodePart5}{CodePart6}";
            bool isCodeValid = false;

            if (fullCode.Length == 6)
            {
                isCodeValid = await _changePasswordServiceClient.SendVerificationCodeAsync(RecoveryMail, fullCode);
            }

            if (isCodeValid)
            {
                IsCodeVerificationVisible = false;
                IsChangePasswordVisible = true;
            }
            else
            {
                CodeErrorMessage = Properties.Resources.InvalidCode_Label;
            }
        }

        private void CancelVerification(object obj)
        {
            IsCodeVerificationVisible = false;
            EmptyFields();
        }

        private void ExecuteSignUpCommand(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new SignUpViewModel(_mainWindowViewModel));
        }

        private bool CanExecuteLogInCommand(object obj)
        {
            if (string.IsNullOrEmpty(Mail) || string.IsNullOrEmpty(Password))
            {
                return false;
            }

            return true;
        }

        private async void ExecuteLogInCommand(object obj)
        {
            string hashedPassword = HashPassword(Password);

            try
            {
                await _logInServiceClient.LogInAsync(Mail, hashedPassword);
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error with the login service.", cex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while communicating with the login service.", tex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while logging in.", ex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
        }

        private void ExcuteLogInAsInvitedCommand(object obj)
        {
            try
            {
                PlayerSingleton.Instance.LogIn(MappingInvited());
                _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private void ExecuteCloseServerError(object obj)
        {
            IsServiceErrorVisible = false;
        }

        private void CloseChangedPasswordMessage(object obj)
        {
            PasswordChangedSuccesfully = false;

            EmptyFields();
        }

        public void LogInResult(OperationResult result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (result.IsSuccess)
                {
                    _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
                }
                else if (result.Message == "Access denied: This account has been reported too many times.")
                {
                    ErrorMessage = Properties.Resources.BannedAccount_Label;
                }
                else if (result.IsDataBaseError)
                {
                    ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                    IsServiceErrorVisible= true;
                }
                else
                {
                    ErrorMessage = Properties.Resources.NonexistentAccount_Label;
                }
            });
        }

        public void AccountInfo(PlayerDTO player)
        {
            Player playerInstance = MappingPlayer(player);
            PlayerSingleton.Instance.LogIn(playerInstance);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                StringBuilder stringBuilder = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        private Player MappingPlayer(PlayerDTO playerDTO)
        {
            Player player = new Player
            {
                Name = playerDTO.Name,
                Id = playerDTO.Id,
                LabelPath = playerDTO.LabelPath,
                PicturePath = playerDTO.PicturePath,
                AccountId = playerDTO.AccountId
            };

            return player;
        }

        private Player MappingInvited()
        {
            Player player = new Player
            {
                Name = "Invited",
                Id = 0,
                LabelPath = Properties.Resources.Apprentice_Label,
                PicturePath = "pack://application:,,,/Assets/Images/ProfilePictures/Picture1.png",
            };

            return player;
        }

        public void ChangePasswordResult(OperationResult result)
        {
            if (result.IsSuccess)
            {
                Password = EditedPassword;
            }
            else if (result.IsDataBaseError)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
            else
            {
                EmailErrorMessage = Properties.Resources.NonexistentAccount_Label;
            }
        }

        public async void ObtainVerificationCodeClient()
        {
            try
            {
                bool isMailValid = await _changePasswordServiceClient.ObtainVerificationCodeAsync(RecoveryMail);

                if (!isMailValid)
                {
                    EmailErrorMessage = Properties.Resources.NonexistentAccount_Label;
                    return;
                }
                else
                {
                    IsForgotPasswordVisible = false;
                    IsCodeVerificationVisible = true;
                }
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while obtaining verification code.", cex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while getting verification code.", tex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while getting verification code.", ex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
        }

        public async void ChangePasswordClient()
        {
            try
            {
                if (!Validations.IsValidPassword(EditedPassword))
                {
                    PasswordErrorMessage = Properties.Resources.InvalidPassword_Label;
                }
                else
                {
                    string hashedPassword = HashPassword(EditedPassword);
                    await _changePasswordServiceClient.SendNewPasswordAsync(RecoveryMail, hashedPassword);

                    IsChangePasswordVisible = false;
                    PasswordChangedSuccesfully = true;
                }
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while changing password.", cex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while changing password.", tex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while changing password.", ex);
                IsServiceErrorVisible = true;
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
            }
        }

        private void EmptyFields()
        {
            RecoveryMail = string.Empty;
            EditedPassword = string.Empty;
            CodeErrorMessage = string.Empty;
            EmailErrorMessage = string.Empty;
            PasswordErrorMessage = string.Empty;
            CodePart1 = string.Empty;
            CodePart2 = string.Empty;
            CodePart3 = string.Empty;
            CodePart4 = string.Empty;
            CodePart5 = string.Empty;
            CodePart6 = string.Empty;

            ClearPasswordBox?.Invoke();
        }
    }
}
