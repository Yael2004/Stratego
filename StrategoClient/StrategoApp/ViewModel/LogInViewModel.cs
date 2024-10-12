using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class LogInViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private bool _isViewEnabled;
        private string _errorMessage;
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

        public ICommand LogInCommand { get; }
        public ICommand SignUpCommand { get; }

        public LogInViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
        }

        public LogInViewModel()
        {
            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
        }

        private void ExecuteSignUpCommand()
        {
            throw new NotImplementedException();
        }

        private bool CanExecuteLogInCommand(object obj)
        {
            return true;
        }

        private void ExecuteLogInCommand(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }
    }
}
