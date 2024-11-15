using StrategoApp.Model;
using StrategoApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategoApp.View
{
    public partial class Game : UserControl
    {
        private Button[,] _boardButtons;
        private Cell selectedCell = null;
        private Dictionary<Piece, Queue<(int row, int column)>> pieceMovementHistory = new Dictionary<Piece, Queue<(int, int)>>();
        public Game()
        {
            InitializeComponent();
        }

        public bool IsValidMove(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            var viewModel = DataContext as GameViewModel;

            if (viewModel.invalidPositions.Contains((destinationCell.Row, destinationCell.Column)))
            {
                MessageBox.Show("Movimiento no permitido en el área central.");
                return false;
            }

            bool isAdjacent = Math.Abs(destinationCell.Row - originCell.Row) + Math.Abs(destinationCell.Column - originCell.Column) == 1;

            if (movingPiece.Id == 0)
            {
                bool isClearPath = IsPathClear(originCell, destinationCell);
                return isClearPath;
            }

            return isAdjacent;
        }


        public bool IsPathClear(Cell originCell, Cell destinationCell)
        {
            var viewModel = DataContext as GameViewModel;
            if (originCell.Row == destinationCell.Row)
            {
                int minCol = Math.Min(originCell.Column, destinationCell.Column);
                int maxCol = Math.Max(originCell.Column, destinationCell.Column);
                for (int col = minCol + 1; col < maxCol; col++)
                {
                    var cell = viewModel.Board.FirstOrDefault(c => c.Row == originCell.Row && c.Column == col);
                    if (cell != null && cell.IsOccupied)
                        return false;
                }
            }
            else if (originCell.Column == destinationCell.Column)
            {
                int minRow = Math.Min(originCell.Row, destinationCell.Row);
                int maxRow = Math.Max(originCell.Row, destinationCell.Row);
                for (int row = minRow + 1; row < maxRow; row++)
                {
                    var cell = viewModel.Board.FirstOrDefault(c => c.Row == row && c.Column == originCell.Column);
                    if (cell != null && cell.IsOccupied)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool IsValidRepeatedMove(Piece piece, Cell destinationCell)
        {
            if (!pieceMovementHistory.ContainsKey(piece))
                pieceMovementHistory[piece] = new Queue<(int, int)>();

            var history = pieceMovementHistory[piece];

            history.Enqueue((destinationCell.Row, destinationCell.Column));

            if (history.Count > 3)
                history.Dequeue();

            if (history.Count == 3)
            {
                var moves = history.ToArray();
                if ((moves[0].Item1 == moves[2].Item1 && moves[0].Item2 == moves[2].Item2) &&
                    (moves[1].Item1 == destinationCell.Row && moves[1].Item2 == destinationCell.Column))
                {
                    MessageBox.Show("Movimiento repetitivo no permitido.");
                    return false;
                }

            }

            return true;
        }

        public void OnCellClicked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as GameViewModel;
            if (viewModel == null) return;

            var button = sender as Button;
            if (button == null) return;

            var cell = button.DataContext as Cell;
            if (cell == null) return;

            if (selectedCell == null && cell.IsOccupied)
            {
                selectedCell = cell;
            }
            else if (selectedCell != null)
            {
                Piece movingPiece = selectedCell.OccupyingPiece;

                if (cell.IsOccupied && cell.OccupyingPiece != null && cell.OccupyingPiece.Id == movingPiece.Id)
                {
                    MessageBox.Show("No puedes moverte a una celda ocupada por otra de tus propias piezas.");
                    selectedCell = null;
                    return;
                }

                bool isValidMove = IsValidMove(selectedCell, cell, movingPiece)
                                   && IsValidRepeatedMove(movingPiece, cell);

                if (isValidMove)
                {
                    cell.OccupiedPieceImage = selectedCell.OccupiedPieceImage;
                    cell.IsOccupied = true;
                    cell.OccupyingPiece = selectedCell.OccupyingPiece;

                    selectedCell.OccupiedPieceImage = null;
                    selectedCell.IsOccupied = false;
                    selectedCell.OccupyingPiece = null;

                    viewModel.SendUpdatedPositionToServer(selectedCell.Row, selectedCell.Column, cell.Row, cell.Column);

                    selectedCell = null;
                }
                else
                {
                    selectedCell = null;
                    MessageBox.Show("Movimiento inválido.");
                }
            }
        }

    }
}
