using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StrategoApp.View
{
    /// <summary>
    /// Lógica de interacción para Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        public Game()
        {
            InitializeComponent();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 100; i++)
            {
                Button cellButton = new Button
                {
                    Background = Brushes.Transparent,
                    Margin = new Thickness(1)
                };

                cellButton.Click += CellButton_Click;
                BoardGrid.Children.Add(cellButton);
            }
        }

        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
        }

    }
}
