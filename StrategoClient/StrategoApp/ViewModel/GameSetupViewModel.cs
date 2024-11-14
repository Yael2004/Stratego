using StrategoApp.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StrategoApp.ViewModel
{
    public class GameSetupViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainWindowViewModel;
        public ObservableCollection<Piece> AvailablePieces { get; set; }
        public ObservableCollection<Cell> PlayerBoard { get; set; }
        public ICommand ConfirmCommand { get; }

        public GameSetupViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            AvailablePieces = new ObservableCollection<Piece>
            {
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/DarkKnight.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Abysswatcher.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Darkwraith.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/GrimReaper.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Hellhound.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Necrofiend.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Nightmare.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/PotionTrap.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Soulhunter.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Archfiend.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Trapbreaker.png")) },
                new Piece { Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Necronomicon.png")) }
            };

            PlayerBoard = new ObservableCollection<Cell>();
            for (int i = 0; i < 40; i++)
            {
                PlayerBoard.Add(new Cell());
            }

            ConfirmCommand = new ViewModelCommand(ConfirmPlacement);
        }

        private void ConfirmPlacement(object obj)
        {
            try
            {
                _mainWindowViewModel.ChangeViewModel(new GameViewModel(_mainWindowViewModel));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }

    public class Piece : ViewModelBase
    {
        public string Name { get; set; }
        public BitmapImage PieceImage { get; set; }
    }
}
