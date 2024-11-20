using StrategoApp.GameService;
using StrategoApp.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StrategoApp.ViewModel
{
    public class GameSetupViewModel : ViewModelBase
    {
        private readonly GameViewModel _gameViewModel;
        private string _counter;
        private MainWindowViewModel _mainWindowViewModel;
        public ObservableCollection<Piece> AvailablePieces { get; set; }
        public ObservableCollection<Cell> PlayerBoard { get; set; }
        //public ICommand ConfirmCommand { get; }

        public GameSetupViewModel(MainWindowViewModel mainWindowViewModel, GameViewModel gameViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _gameViewModel = gameViewModel;

            AvailablePieces = new ObservableCollection<Piece>
            {
                new Piece {Id = 0,  Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/DarkKnight.png")), RemainingQuantity = 3, MaxQuantity = 3 },
                new Piece {Id = 1, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Abysswatcher.png")), RemainingQuantity = 1, MaxQuantity = 1 },
                new Piece {Id = 2, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Darkwraith.png")), RemainingQuantity = 2, MaxQuantity = 2 },
                new Piece {Id = 3, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/GrimReaper.png")), RemainingQuantity = 4, MaxQuantity = 4 },
                new Piece {Id = 4, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Hellhound.png")), RemainingQuantity = 4, MaxQuantity = 4 },
                new Piece {Id = 5, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Necrofiend.png")), RemainingQuantity = 1, MaxQuantity = 1 },
                new Piece {Id = 6, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Nightmare.png")), RemainingQuantity = 7, MaxQuantity = 7 },
                new Piece {Id = 7, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/PotionTrap.png")), RemainingQuantity = 8, MaxQuantity = 8 },
                new Piece {Id = 8, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Soulhunter.png")), RemainingQuantity = 4, MaxQuantity = 4 },
                new Piece {Id = 9, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Archfiend.png")), RemainingQuantity = 1, MaxQuantity = 1 },
                new Piece {Id = 10, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Trapbreaker.png")), RemainingQuantity = 4, MaxQuantity = 4 },
                new Piece {Id = 11, Name = "Dragon", PieceImage = new BitmapImage(new Uri("pack://application:,,,/StrategoApp;component/Assets/Game/Necronomicon.png")), RemainingQuantity = 1, MaxQuantity = 1 }
            };

            PlayerBoard = new ObservableCollection<Cell>();
            int rows = 4;
            int cols = 10;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    PlayerBoard.Add(new Cell { Row = row, Column = col });
                }
            }

            Task.Run(() => ShowCountDown());
        }

        public string Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnPropertyChanged();
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 60; i >= 1; i--)
            {
                Counter = i.ToString();
                await Task.Delay(1000);
            }

            ConfirmPlacement();
        }

        private void ConfirmPlacement()
        {
            var initialPositions = PlayerBoard
                .Where(cell => cell.IsOccupied)
                .Select(cell => new PositionDTO
                {
                    InitialX = cell.Row,
                    InitialY = cell.Column,
                    FinalX = cell.Row,
                    FinalY = cell.Column,
                    PieceId = cell.OccupyingPiece.Id,
                    MoveType = "initial"
                })
                .ToList();

            _gameViewModel.AvailablePices = AvailablePieces;
            _gameViewModel.LoadInitialPositions(initialPositions);
            _mainWindowViewModel.ChangeViewModel(_gameViewModel);
        }

        public void DecrementPieceQuantity(Piece piece)
        {
            piece.RemainingQuantity--;
            if (piece.RemainingQuantity < 0) piece.RemainingQuantity = 0;
            OnPropertyChanged(nameof(AvailablePieces));
        }
    }
}
