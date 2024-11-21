using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace StrategoApp.Model
{
    public class Piece : INotifyPropertyChanged
    {
        private int _remainingQuantity;
        public string Color { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public BitmapImage PieceImage { get; set; }
        public bool IsPlaced { get; set; } = false;
        public int MaxQuantity { get; set; }
        public int RemainingQuantity
        {
            get => _remainingQuantity;
            set
            {
                if (_remainingQuantity != value)
                {
                    _remainingQuantity = value;
                    OnPropertyChanged(nameof(RemainingQuantity));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}