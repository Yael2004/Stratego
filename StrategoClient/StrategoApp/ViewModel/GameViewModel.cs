using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        MainWindowViewModel _mainWindowViewModel;

        public GameViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}
