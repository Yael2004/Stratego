using StrategoApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object _currentViewModel;

        public object CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            CurrentViewModel = new LogInViewModel(this, false);
        }

        public void ChangeViewModel(ViewModelBase newViewModel)
        {
            CurrentViewModel = newViewModel;
        }
    }
}
