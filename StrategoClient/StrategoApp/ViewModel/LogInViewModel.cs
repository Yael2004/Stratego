using StrategoApp.Helpers;
using StrategoApp.LogInService;
using StrategoApp.Model;
using StrategoApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class LogInViewModel : ViewModelBase, LogInService.ILogInServiceCallback
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isPasswordVisible;
        private string _togglePasswordVisibilityIcon;
        public string LogInErrorMessage { get; set; }

        private readonly LogInServiceClient _logInServiceClient;

        private MainWindowViewModel _mainWindowViewModel;

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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogInCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand LogInAsInvitedCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }

        public LogInViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _logInServiceClient = new LogInServiceClient(new System.ServiceModel.InstanceContext(this));
            _mainWindowViewModel = mainWindowViewModel;

            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            LogInAsInvitedCommand = new ViewModelCommand(ExcuteLogInAsInvitedCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
            TogglePasswordVisibilityCommand = new ViewModelCommand(p => ExecuteTogglePasswordVisibilityCommand());
        }

        public LogInViewModel()
        {
            _logInServiceClient = new LogInServiceClient(new System.ServiceModel.InstanceContext(this));

            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
            LogInAsInvitedCommand = new ViewModelCommand(ExcuteLogInAsInvitedCommand);
            TogglePasswordVisibilityCommand = new ViewModelCommand(p => ExecuteTogglePasswordVisibilityCommand());
        }

        private void ExecuteTogglePasswordVisibilityCommand()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void ExecuteSignUpCommand()
        {
            _mainWindowViewModel.ChangeViewModel(new SignUpViewModel(_mainWindowViewModel));
        }

        private bool CanExecuteLogInCommand(object obj)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                return false;
            }
                
            return true;
        }

        private async void ExecuteLogInCommand(object obj)
        {
            try
            {
                await _logInServiceClient.LogInAsync(Username, Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExcuteLogInAsInvitedCommand(object obj)
        {
            try
            {
                _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al conectar con el servidor: {ex.Message}";
            }
        }

        public void LogInResult(OperationResult result)
        {
            if (result.IsSuccess)
            {
                _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            }
            else
            {
                ErrorMessage = Properties.Resources.NonexistentAccount_Label;
            }
        }

        public void AccountInfo(PlayerDTO player)
        {
            PlayerSingleton.Instance.LogIn(player);
        }
    }
}
