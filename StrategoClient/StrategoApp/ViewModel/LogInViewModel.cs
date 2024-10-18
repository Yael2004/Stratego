using StrategoApp.Helpers;
using StrategoApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class LogInViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private bool _isViewEnabled;
        private string _errorMessage;
        public string LogInErrorMessage { get; set; }

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

        public bool IsViewEnabled
        {
            get { return _isViewEnabled; }
            set
            {
                _isViewEnabled = value;
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

        public void SetLogInErrorMessage(string resourceKey)
        {
            LogInErrorMessage = StrategoApp.Properties.Resources.ResourceManager.GetString(resourceKey);
            OnPropertyChanged(nameof(LogInErrorMessage));
        }

        public ICommand LogInCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand LogInAsInvitedCommand { get; }

        public LogInViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            LogInAsInvitedCommand = new ViewModelCommand(ExcuteLogInAsInvitedCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
        }

        public LogInViewModel()
        {
            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
            LogInAsInvitedCommand = new ViewModelCommand(ExcuteLogInAsInvitedCommand);
        }

        private void ExecuteSignUpCommand()
        {
            _mainWindowViewModel.ChangeViewModel(new SignUpViewModel(_mainWindowViewModel));
        }

        private bool CanExecuteLogInCommand(object obj)
        {
            return true;
        }

        private void ExecuteLogInCommand(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        private void ExcuteLogInAsInvitedCommand(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }
    }
}
