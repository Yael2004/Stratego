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
        private bool _isPasswordVisible;
        private string _togglePasswordVisibilityIcon;

        private MainWindowViewModel _mainWindowViewModel;
        private SignUpServiceClient _signUpServiceClient;

        private string _usernameError;
        private string _emailError;
        private string _passwordError;

        public ICommand SignUpCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }

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
                OnPropertyChanged();
                _togglePasswordVisibilityIcon = _isPasswordVisible ? "HidePasswordIcon" : "ShowPasswordIcon";
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

        public SignUpViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            InstanceContext context = new InstanceContext(this);
            _signUpServiceClient = new SignUpServiceClient(context, "NetTcpBinding_ISignUpService");

            SignUpCommand = new ViewModelCommand(ExecuteSignUpCommand, CanExecuteSignUpCommand);
            CancelCommand = new ViewModelCommand(ExecuteCancelCommand);
            TogglePasswordVisibilityCommand = new ViewModelCommand(p => ExecuteTogglePasswordVisibilityCommand());
        }

        private void ExecuteTogglePasswordVisibilityCommand()
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
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
        }

        private void ExecuteCancelCommand(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
        }

        public void SignUpResult(OperationResult result)
        {
            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message);
                _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
            }
            else
            {
                MessageBox.Show(result.Message);
            }
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
    }
}
