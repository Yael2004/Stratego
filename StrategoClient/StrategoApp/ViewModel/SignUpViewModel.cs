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

        private MainWindowViewModel _mainWindowViewModel;
        private SignUpServiceClient _signUpServiceClient;

        private string _usernameError;
        private string _emailError;
        private string _passwordError;

        public ICommand SignUpCommand { get; }
        public ICommand CancelCommand { get; }

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

        public SignUpViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            InstanceContext context = new InstanceContext(this);
            _signUpServiceClient = new SignUpServiceClient(context, "NetTcpBinding_ISignUpService");

            SignUpCommand = new ViewModelCommand(ExecuteSignUpCommand, CanExecuteSignUpCommand);
            CancelCommand = new ViewModelCommand(ExecuteCancelCommand);
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
            ValidateFields();

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
                Console.WriteLine($"Error al crear la cuenta: {result.Message}");
            }
        }

        public void ValidateFields()
        {
            UsernameError = Validations.IsValidUsername(Username) ? string.Empty : Properties.Resources.InvalidUsername_Label;
            EmailError = Validations.IsValidEmail(Email) ? string.Empty : Properties.Resources.InvalidMail_Label;
            PasswordError = Validations.IsValidPassword(Password) ? string.Empty : Properties.Resources.InvalidPassword_Label;
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
