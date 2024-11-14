using StrategoApp.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace StrategoApp.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainWindowViewModel;
        public ObservableCollection<Cell> Board { get; set; }
        public ObservableCollection<Piece> PlayerPieces { get; set; }

        public GameViewModel(MainWindowViewModel mainWindowViewModel)
        {
            Board = new ObservableCollection<Cell>();
            PlayerPieces = new ObservableCollection<Piece>();
            InitializeBoard();
            InitializePlayerPieces();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 144; i++)
            {
                Board.Add(new Cell());
            }
        }

        private void InitializePlayerPieces()
        {
            for (int i = 0; i < 12; i++)
            {
                PlayerPieces.Add(new Piece
                {
                    Name = $"Piece {i + 1}",
                    PieceImage = new BitmapImage(new Uri($"pack://application:,,,/StrategoApp;component/Assets/Game/Dragon.png"))
                    //PieceImage = new BitmapImage(new Uri($"pack://application:,,,/StrategoApp;component/Assets/Game/Piece{i + 1}.png"))
                });
            }
        }
    }
}
