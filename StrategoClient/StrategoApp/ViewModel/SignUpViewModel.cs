using log4net;
using StrategoApp.Helpers;
using StrategoApp.LogInService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class SignUpViewModel : ViewModelBase, LogInService.ISignUpServiceCallback
    {
        private static readonly ILog Log = Log<SignUpViewModel>.GetLogger();

        private string _username;
        private string _password;
        private string _email;
        private string _usernameError;
        private string _emailError;
        private string _passwordError;
        private string _signUpResultMessage;
        private bool _isServiceErrorVisible;
        private bool _isPasswordVisible;
        private bool _isSignUpResultVisible;

        private readonly SignUpServiceClient _signUpServiceClient;

        private readonly MainWindowViewModel _mainWindowViewModel;

        public ICommand SignUpCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }
        public ICommand AcceptSuccessfullyMessage { get; }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
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

        public string UsernameError
        {
            get { return _usernameError; }
            set
            {
                _usernameError = value;
                OnPropertyChanged();
            }
        }

        public string EmailError
        {
            get { return _emailError; }
            set
            {
                _emailError = value;
                OnPropertyChanged();
            }
        }

        public string PasswordError
        {
            get { return _passwordError; }
            set
            {
                _passwordError = value;
                OnPropertyChanged();
            }
        }

        public bool IsPasswordVisible
        {
            get { return _isPasswordVisible; }
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged(nameof(IsPasswordVisible));
            }
        }

        public bool IsSignUpResultVisible
        {
            get { return _isSignUpResultVisible; }
            set
            {
                _isSignUpResultVisible = value;
                OnPropertyChanged();
            }
        }

        public string SignUpResultMessage
        {
            get { return _signUpResultMessage; }
            set
            {
                _signUpResultMessage = value;
                OnPropertyChanged();
            }
        }

        public SignUpViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _signUpServiceClient = new SignUpServiceClient(new InstanceContext(this));

            _mainWindowViewModel = mainWindowViewModel;

            SignUpCommand = new ViewModelCommand(ExecuteSignUpCommand, CanExecuteSignUpCommand);
            CancelCommand = new ViewModelCommand(ExecuteCancelCommand);
            TogglePasswordVisibilityCommand = new ViewModelCommand(ExecuteTogglePasswordVisibilityCommand);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(ExecuteCloseServiceError);
            AcceptSuccessfullyMessage = new ViewModelCommand(AcceptSignUpSuccess);

            IsServiceErrorVisible = false;
            IsPasswordVisible = false;
            IsSignUpResultVisible = false;
        }

        private void ExecuteTogglePasswordVisibilityCommand(object obj)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private bool CanExecuteSignUpCommand(object obj)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Email))
            {
                return false;
            }

            return true;
        }

        private async void ExecuteSignUpCommand(object obj)
        {
            if (ValidateFields())
            {
                string hashedPassword = HashPassword(Password);

                try
                {
                    await _signUpServiceClient.SignUpAsync(Email, hashedPassword, Username);
                }
                catch (CommunicationException cex)
                {
                    Log.Error("Communication error with the signup service.", cex);
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error("Timed out while communicating with the signup service.", tex);
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while signing up.", ex);
                    IsServiceErrorVisible = true;
                }
            }
        }

        private void ExecuteCancelCommand(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
        }

        private void ExecuteCloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
        }

        public void SignUpResult(OperationResult result)
        {
            if (result.IsSuccess)
            {
                SignUpResultMessage = Properties.Resources.SignUpMessage_Label;
            }
            else
            {
                SignUpResultMessage = Properties.Resources.AccountAlreadyExistMessage_Label;
            }

            IsSignUpResultVisible = true;
        }

        public bool ValidateFields()
        {
            bool isValid = true;

            isValid &= IsValidUsername();
            isValid &= IsValidEmail();
            isValid &= IsValidPassword();

            return isValid;
        }

        private bool IsValidUsername()
        {
            Username = Username.Trim();

            if (!Validations.IsValidUsername(Username))
            {
                UsernameError = Properties.Resources.InvalidUsername_Label;

                return false;
            }

            UsernameError = string.Empty;

            return true;
        }

        private bool IsValidEmail()
        {
            Email = Email.Trim();

            if (!Validations.IsValidEmail(Email))
            {
                EmailError = Properties.Resources.InvalidMail_Label;

                return false;
            }

            EmailError = string.Empty;

            return true;
        }

        private bool IsValidPassword()
        {
            if (!Validations.IsValidPassword(Password))
            {
                PasswordError = Properties.Resources.InvalidPassword_Label;

                return false;
            }

            PasswordError = string.Empty;

            return true;
        }

        private void AcceptSignUpSuccess(object obj)
        {
            IsSignUpResultVisible = false;

            if (SignUpResultMessage == Properties.Resources.SignUpMessage_Label)
            {
                _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
            }
        }

        private static string HashPassword(string password)
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
    }
}
