using StrategoApp.Model;
using StrategoApp.ViewModel;
using System.Windows.Media.Imaging;

namespace StrategoApp.ViewModel
{
    public class Cell : ViewModelBase
    {
        public int Row { get; set; }
        public int Column { get; set; }

        private bool _isOccupied;
        public bool IsOccupied
        {
            get => _isOccupied;
            set
            {
                _isOccupied = value;
                OnPropertyChanged();
            }
        }

        private bool _isObstacle;
        public bool IsObstacle
        {
            get => _isObstacle;
            set
            {
                _isObstacle = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _occupiedPieceImage;
        public BitmapImage OccupiedPieceImage
        {
            get => _occupiedPieceImage;
            set
            {
                _occupiedPieceImage = value;
                OnPropertyChanged();
            }
        }

        private Piece _occupyingPiece;
        public Piece OccupyingPiece
        {
            get => _occupyingPiece;
            set
            {
                _occupyingPiece = value;
                OnPropertyChanged();
            }
        }
    }
}
