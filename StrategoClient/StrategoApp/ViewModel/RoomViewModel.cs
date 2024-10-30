using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class RoomViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        public ICommand ExecuteBackToLobbyCommand { get; }
        public RoomViewModel(MainWindowViewModel mainWindowViewModel)
        {
            ExecuteBackToLobbyCommand = new ViewModelCommand(ExecuteBackToLobby);
            _mainWindowViewModel = mainWindowViewModel;

        }

        private void ExecuteBackToLobby(Object obj)
        {
            try
            {
                _mainWindowViewModel.ChangeViewModel(LobbyViewModel.Instance(_mainWindowViewModel));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
