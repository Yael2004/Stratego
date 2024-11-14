using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StrategoApp.ViewModel;

namespace StrategoApp.View
{
    public partial class Game : UserControl
    {
        public Game()
        {
            InitializeComponent();
        }

        private void OnPieceDragStart(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Image draggedImage)
            {
                DragDrop.DoDragDrop(draggedImage, draggedImage.Source, DragDropEffects.Move);
            }
        }

        private void OnPieceDropped(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap) && sender is Button targetCell)
            {
                var droppedImage = e.Data.GetData(DataFormats.Bitmap) as BitmapImage;
                targetCell.Background = new ImageBrush(droppedImage);
                targetCell.IsEnabled = false; // Impide colocar otra ficha en la misma celda
            }
        }

        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes implementar la lógica que deseas al hacer clic en las celdas
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Ejemplo de cómo manejar el clic en una celda
                MessageBox.Show("Celda seleccionada: " + clickedButton.Tag);
            }
        }

    }
}
