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
    public class SignUpViewModel : ViewModelBase, LogInService.ILogInServiceCallback
    {
        private string _username;
        private string _password;
        private string _email;

        private MainWindowViewModel _mainWindowViewModel;
        private LogInServiceClient _logInServiceClient;

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

        public SignUpViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            InstanceContext context = new InstanceContext(this);
            _logInServiceClient = new LogInServiceClient(context, "NetTcpBinding_ILogInService");

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

        private void ExecuteSignUpCommand(object obj)
        {
            try
            {
                _logInServiceClient.SignUp(Email, Password, Username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de conexión: {ex.Message}");
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

        public void LogInResult(OperationResult result)
        {
            throw new NotImplementedException();
        }
    }
}
