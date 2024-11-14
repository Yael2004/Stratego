using System.Windows.Media.Imaging;

namespace StrategoApp.Model
{
    public class Piece
    {
        public string Name { get; set; }
        public BitmapImage PieceImage { get; set; }
        public bool IsPlaced { get; set; } = false;
    }
}