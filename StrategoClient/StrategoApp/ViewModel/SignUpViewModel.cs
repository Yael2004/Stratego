using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class SignUpViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private string _email;

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
        }

        ICommand SignUpCommand { get; }
        ICommand CancelCommand { get; }

        private bool CanExecuteSignUpCommand()
        {
            return true;
        }

        private void ExecuteSignUpCommand()
        {

        }

        private void ExecuteCancelCommand()
        {
            _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
        }
    }
}
