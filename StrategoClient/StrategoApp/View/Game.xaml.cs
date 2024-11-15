using StrategoApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategoApp.View
{
    public partial class Game : UserControl
    {
        private Button[,] _boardButtons;
        public Game()
        {
            InitializeComponent();
            //InitializeBoard();
        }

        //private void InitializeBoard()
        //{
        //    _boardButtons = new Button[10, 10];
        //    var uniformGrid = new UniformGrid { Rows = 10, Columns = 10 };

        //    for (int row = 0; row < 10; row++)
        //    {
        //        for (int column = 0; column < 10; column++)
        //        {
        //            var cellButton = new Button
        //            {
        //                Background = Brushes.Transparent,
        //                BorderBrush = Brushes.Black,
        //                BorderThickness = new Thickness(1),
        //                Tag = new Tuple<int, int>(row, column)
        //            };

        //            cellButton.Click += CellButton_Click;
        //            uniformGrid.Children.Add(cellButton);
        //            _boardButtons[row, column] = cellButton;
        //        }
        //    }

        //    BoardGrid.Children.Add(uniformGrid);
        //}

        public void SetInitialPositions(IEnumerable<(int Row, int Column, BitmapImage PieceImage)> initialPositions)
        {
            foreach (var (row, column, pieceImage) in initialPositions)
            {
                var cellButton = _boardButtons[row, column];
                cellButton.Background = new ImageBrush(pieceImage);
                cellButton.IsEnabled = false;
            }
        }
        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                var row = Grid.GetRow(clickedButton);
                var column = Grid.GetColumn(clickedButton);
                var cellPosition = Tuple.Create(row, column);

                var viewModel = DataContext as GameViewModel;
                viewModel?.SendPositionCommand.Execute(cellPosition);
            }
        }

    }
}
