using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.ViewModel
{
    internal class LobbyViewModel : ViewModelBase
    {
        private bool _isViewEnabled;
        private string _errorMessage;
        private MainWindowViewModel _mainWindowViewModel;

        public LobbyViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
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
    }
}
