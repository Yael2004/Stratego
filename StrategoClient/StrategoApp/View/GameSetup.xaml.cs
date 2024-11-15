using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StrategoApp.Model;
using StrategoApp.ViewModel;

namespace StrategoApp.View
{
    public partial class GameSetup : UserControl
    {
        public GameSetup()
        {
            InitializeComponent();
        }

        private void OnPieceSelected(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && DataContext is GameSetupViewModel viewModel)
            {
                var piece = image.DataContext as Piece;
                if (piece != null && piece.RemainingQuantity > 0)
                {
                    DragDrop.DoDragDrop(image, piece, DragDropEffects.Move);
                }
            }
        }

        private void OnPieceDropped(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Piece)) && sender is Button targetButton)
            {
                var piece = e.Data.GetData(typeof(Piece)) as Piece;
                if (piece != null && DataContext is GameSetupViewModel viewModel)
                {
                    var cell = targetButton.DataContext as Cell;
                    if (cell != null && !cell.IsOccupied && piece.RemainingQuantity > 0)
                    {
                        cell.OccupiedPieceImage = piece.PieceImage;
                        cell.IsOccupied = true;
                        cell.OccupyingPiece = piece;

                        piece.RemainingQuantity--;
                    }
                }
            }
        }

        private void OnPieceRemoved(Cell cell)
        {
            if (cell.IsOccupied && cell.OccupyingPiece != null)
            {
                cell.OccupyingPiece.RemainingQuantity--;
                cell.OccupiedPieceImage = null;
                cell.IsOccupied = false;
                cell.OccupyingPiece = null;
            }
        }
    }
}
