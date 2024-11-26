using StrategoApp.GameService;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using StrategoApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategoApp.ViewModel
{
    public class GameViewModel : ViewModelBase, GameService.IGameServiceCallback, ProfileService.IOtherProfileDataServiceCallback
    {
        private string _username;
        private int _userId;
        private string _profilePicture;
        private string _opponentUsername;
        private int _opponentId;
        private string _opponentProfilePicture;
        private int _selectedPieceId;
        private int _gameId;
        private Cell _selectedCell;

        private MainWindowViewModel _mainWindowViewModel;
        private GameServiceClient _gameServiceClient;
        public ObservableCollection<Piece> PlayerPieces { get; set; }
        public ObservableCollection<Cell> Board { get; set; }
        private Button[,] _boardButtons;
        private Dictionary<Piece, Queue<(int row, int column)>> pieceMovementHistory = new Dictionary<Piece, Queue<(int, int)>>();

        public readonly List<(int Row, int Column)> invalidPositions = new List<(int Row, int Column)> { };

        public ObservableCollection<Piece> AvailablePices { get; set; }

        public ICommand SendPositionCommand { get; }
        public ICommand CellClickedCommand { get; }

        public GameViewModel(MainWindowViewModel mainWindowViewModel, ObservableCollection<Piece> availablePieces)
        {
            invalidPositions.Add((5, 3));
            invalidPositions.Add((5, 4));
            invalidPositions.Add((6, 3));
            invalidPositions.Add((6, 4));
            invalidPositions.Add((5, 7));
            invalidPositions.Add((5, 8));
            invalidPositions.Add((6, 7));
            invalidPositions.Add((6, 8));

            _mainWindowViewModel = mainWindowViewModel;
            AvailablePices = availablePieces;
            _gameServiceClient = new GameServiceClient(new System.ServiceModel.InstanceContext(this));

            PlayerPieces = new ObservableCollection<Piece>();
            Board = new ObservableCollection<Cell>();
            CellClickedCommand = new ViewModelCommandGeneric<Cell>(OnCellClicked);
            LoadPlayerData();
            InitializeBoard();
        }

        public GameViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _gameServiceClient = new GameServiceClient(new System.ServiceModel.InstanceContext(this));
            _mainWindowViewModel = mainWindowViewModel;
            PlayerPieces = new ObservableCollection<Piece>();
            Board = new ObservableCollection<Cell>();
            CellClickedCommand = new ViewModelCommandGeneric<Cell>(OnCellClicked);
            LoadPlayerData();
            InitializeBoard();
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string ProfilePicture
        {
            get => _profilePicture;
            set
            {
                _profilePicture = value;
                OnPropertyChanged();
            }
        }

        public string OpponentUsername
        {
            get => _opponentUsername;
            set
            {
                _opponentUsername = value;
                OnPropertyChanged();
            }
        }

        public int OpponentId
        {
            get => _opponentId;
            set
            {
                _opponentId = value;
                OnPropertyChanged();
            }
        }

        public string OpponentProfilePicture
        {
            get => _opponentProfilePicture;
            set
            {
                _opponentProfilePicture = value;
                OnPropertyChanged();
            }
        }

        public int SelectedPieceId
        {
            get => _selectedPieceId;
            set
            {
                _selectedPieceId = value;
                OnPropertyChanged();
            }
        }

        public Cell SelectedCell
        {
            get => _selectedCell;
            set
            {
                _selectedCell = value;
                OnPropertyChanged();
            }
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    var cell = new Cell
                    {
                        Row = row,
                        Column = col,
                        IsOccupied = false,
                        OccupiedPieceImage = null,
                        OccupyingPiece = null
                    };
                
                    if (row >= 0 && row < 4)
                    {
                        if (cell.OccupyingPiece == null)
                        {
                            cell.IsOccupied = true;
                            cell.OccupiedPieceImage = new BitmapImage(new Uri($"pack://application:,,,/StrategoApp;component/Assets/Game/Dragon.png"));
                            cell.OccupyingPiece = new Piece
                            {
                                Name = "Dragon",
                                Color = "Red"
                            };
                        }
                    }

                    Board.Add(cell);
                }
            }
        }

        private void LoadPlayerData()
        {
            if (PlayerSingleton.Instance.IsLoggedIn())
            {
                Username = PlayerSingleton.Instance.Player.Name;
                UserId = PlayerSingleton.Instance.Player.Id;
                ProfilePicture = PlayerSingleton.Instance.Player.PicturePath;
            }
        }

        public void LoadInitialPositions(List<PositionDTO> initialPositions)
        {
            foreach (var position in initialPositions)
            {
                int adjustedRow = position.InitialX + 6;
                int adjustedColumn = position.InitialY;

                if (adjustedRow >= 0 && adjustedRow < 10 && adjustedColumn >= 0 && adjustedColumn < 10)
                {
                    var cell = Board.FirstOrDefault(c => c.Row == adjustedRow && c.Column == adjustedColumn);

                    if (cell != null)
                    {
                        var piece = AvailablePices.FirstOrDefault(p => p.PowerLevel == position.PowerLevel);

                        if (piece != null)
                        {
                            cell.OccupiedPieceImage = piece.PieceImage;
                            cell.IsOccupied = true;
                            cell.OccupyingPiece = piece;
                            piece.OwnerId = UserId;
                        }
                    }
                }
            }
        }

        public bool IsValidMove(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            if (invalidPositions.Contains((destinationCell.Row, destinationCell.Column)))
            {
                MessageBox.Show("Movimiento no permitido en el área central.");
                return false;
            }

            if (movingPiece.Name == "PotionTrap" || movingPiece.Name == "Necronomicon")
            {
                SelectedCell = null;
                return false;
            }

            bool isAdjacent = Math.Abs(destinationCell.Row - originCell.Row) + Math.Abs(destinationCell.Column - originCell.Column) == 1;

            if (movingPiece.PowerLevel == 0)
            {
                bool isClearPath = IsPathClear(originCell, destinationCell);
                return isClearPath;
            }

            if (destinationCell.IsOccupied && destinationCell.OccupyingPiece != null && destinationCell.OccupyingPiece.Color == movingPiece.Color)
            {
                MessageBox.Show("No puedes moverte a una celda ocupada por otra de tus propias piezas.");
                SelectedCell = null;
                return false;
            }

            return isAdjacent;
        }


        public bool IsPathClear(Cell originCell, Cell destinationCell)
        {
            if (originCell.Row == destinationCell.Row)
            {
                int minCol = Math.Min(originCell.Column, destinationCell.Column);
                int maxCol = Math.Max(originCell.Column, destinationCell.Column);
                for (int col = minCol + 1; col < maxCol; col++)
                {
                    var cell = Board.FirstOrDefault(c => c.Row == originCell.Row && c.Column == col);
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
                    var cell = Board.FirstOrDefault(c => c.Row == row && c.Column == originCell.Column);
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

        public void OnCellClicked(Cell clickedCell)
        {
            if (clickedCell == null) return;

            if (SelectedCell == null && clickedCell.IsOccupied && clickedCell.OccupyingPiece?.OwnerId == UserId)
            {
                SelectedCell = clickedCell;
            }
            else if (SelectedCell != null)
            {
                Piece movingPiece = SelectedCell.OccupyingPiece;

                bool isValidMove = IsValidMove(SelectedCell, clickedCell, movingPiece) && IsValidRepeatedMove(movingPiece, clickedCell);

                if (isValidMove)
                {
                    HandleMove(SelectedCell, clickedCell, movingPiece);
                }
                else
                {
                    MessageBox.Show("Movimiento inválido.");
                }

                SelectedCell = null;
            }
        }

        private void HandleMove(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            SendUpdatedPositionToServer(originCell.Row, originCell.Column, destinationCell.Row, destinationCell.Column);
        }

        private void HandleTrapbreakerRule(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            if (destinationCell.OccupyingPiece?.Name == "PotionTrap" && movingPiece.Name == "Trapbreaker")
            {
                MessageBox.Show("¡El Trapbreaker desactivó una poción!");
                destinationCell.OccupiedPieceImage = originCell.OccupiedPieceImage;
                destinationCell.IsOccupied = true;
                destinationCell.OccupyingPiece = movingPiece;

                originCell.OccupiedPieceImage = null;
                originCell.IsOccupied = false;
                originCell.OccupyingPiece = null;

                SendUpdatedPositionToServer(originCell.Row, originCell.Column, destinationCell.Row, destinationCell.Column);
            }
        }

        private void HandleAbysswatcherRule(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            var defenderPiece = destinationCell.OccupyingPiece;

            if (movingPiece.Name == "Abysswatcher" && defenderPiece?.Name == "Archfiend")
            {
                MessageBox.Show("¡El Abysswatcher eliminó al Archfiend!");
                destinationCell.OccupiedPieceImage = originCell.OccupiedPieceImage;
                destinationCell.IsOccupied = true;
                destinationCell.OccupyingPiece = movingPiece;

                originCell.OccupiedPieceImage = null;
                originCell.IsOccupied = false;
                originCell.OccupyingPiece = null;

                SendUpdatedPositionToServer(originCell.Row, originCell.Column, destinationCell.Row, destinationCell.Column);
            }
        }

        public void SendUpdatedPositionToServer(int initialRow, int initialColumn, int targetRow, int targetColumn)
        {
            var position = new PositionDTO
            {
                InitialX = initialRow,
                InitialY = initialColumn,
                FinalX = targetRow,
                FinalY = targetColumn,
                PowerLevel = Board.FirstOrDefault(cell => cell.Row == targetRow && cell.Column == targetColumn)?.OccupyingPiece?.PowerLevel ?? 0,
            };

            SendPositionToServer(position);
        }

        private async void SendPositionToServer(PositionDTO position)
        {
            try
            {
                await _gameServiceClient.SendPositionAsync(_gameId, UserId, position);
                Console.WriteLine($"Posición enviada: Fila {position.FinalX}, Columna {position.FinalY}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar la posición: {ex.Message}");
            }
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                Username = response.PlayerInfo.PlayerInfo.Name;
                UserId = response.PlayerInfo.PlayerInfo.Id;
                ProfilePicture = response.PlayerInfo.PlayerInfo.PicturePath;
            }
        }

        public void OnGameStarted(int gameId, GameService.OperationResult operationResult)
        {
            _gameId = gameId;
            if (operationResult.IsSuccess)
            {
                _mainWindowViewModel.ChangeViewModel(new GameSetupViewModel(_mainWindowViewModel, this));
            }
            else
            {
                MessageBox.Show("Error starting game: " + operationResult.Message);
            }
        }

        public void OnReceiveOpponentPosition(PositionDTO position, GameService.OperationResult operationResult)
        {
            if (!operationResult.IsSuccess)
            {
                MessageBox.Show($"Error del servidor: {operationResult.Message}");
                return;
            }

            try
            {
                int invertedInitialRow = Math.Abs(9 - position.InitialX);
                int invertedFinalRow = Math.Abs(9 - position.FinalX);

                var originCell = Board.FirstOrDefault(c => c.Row == invertedInitialRow && c.Column == position.InitialY);
                var destinationCell = Board.FirstOrDefault(c => c.Row == invertedFinalRow && c.Column == position.FinalY);

                if (originCell == null || destinationCell == null)
                {
                    MessageBox.Show("Error al procesar las celdas de origen o destino.");
                    return;
                }

                var movementInstruction = new MovementInstructionDTO
                {
                    DefenderId = UserId,
                    Result = ProcessMove(originCell, destinationCell, position.PowerLevel)
                };

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _gameServiceClient.SendMovementInstructionsAsync(_gameId, movementInstruction);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al enviar las instrucciones de movimiento: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error procesando la posición del oponente: {ex.Message}");
            }
        }


        private string ProcessMove(Cell originCell, Cell destinationCell, int attackerPowerLevel)
        {
            var defenderPiece = destinationCell.OccupyingPiece;

            if (destinationCell.IsOccupied && defenderPiece != null && defenderPiece.OwnerId == UserId)
            {
                if (attackerPowerLevel > defenderPiece.PowerLevel)
                {
                    MessageBox.Show("¡El oponente destruyó tu pieza!");

                    UpdateCellState(originCell, null, false, null);
                    UpdateCellState(destinationCell, new BitmapImage(new Uri($"pack://application:,,,/StrategoApp;component/Assets/Game/Dragon.png")), true, new Piece
                    {
                        Name = "Dragon",
                        Color = "Red"
                    });

                    return "Kill";
                }
                else if (attackerPowerLevel < defenderPiece.PowerLevel)
                {
                    MessageBox.Show("¡Tu pieza resistió el ataque!");

                    UpdateCellState(originCell, null, false, null);
                    return "Fail";
                }
                else
                {
                    MessageBox.Show("¡Empate! Ninguna pieza se movió.");
                    return "Draw";
                }
            }
            else
            {
                UpdateCellState(originCell, null, false, null);
                UpdateCellState(destinationCell, new BitmapImage(new Uri($"pack://application:,,,/StrategoApp;component/Assets/Game/Dragon.png")), true, new Piece
                {
                    Name = "Dragon",
                    Color = "Red"
                });

                return "Move";
            }
        }

        /*
        private void UpdateCellState(Cell cell, BitmapImage pieceImage, bool isOccupied, Piece occupyingPiece)
        {
            cell.OccupiedPieceImage = pieceImage;
            cell.IsOccupied = isOccupied;
            cell.OccupyingPiece = occupyingPiece;
        }
        */

        public void OnOpponentAbandonedGame(GameService.OperationResult operationResult)
        {
            throw new NotImplementedException();
        }

        public void OnGameEnded(string resultString, GameService.OperationResult operationResult)
        {
            throw new NotImplementedException();
        }

        public async void SuscribeToGame(int gameId)
        {
            await _gameServiceClient.JoinGameSessionAsync(gameId, UserId);
        }

        public void OnReceiveMovementInstructions(MovementInstructionResponse movementInstructionResponse)
        {
            if (movementInstructionResponse == null || movementInstructionResponse.OperationResult == null)
            {
                MessageBox.Show("Error: La respuesta del movimiento es nula o inválida.");
                return;
            }

            if (!movementInstructionResponse.OperationResult.IsSuccess)
            {
                MessageBox.Show($"Error en el movimiento: {movementInstructionResponse.OperationResult.Message}");
                return;
            }

            var instruction = movementInstructionResponse.MovementInstructionDTO;
            if (instruction == null)
            {
                MessageBox.Show("Error: Los datos de la instrucción de movimiento son nulos.");
                return;
            }

            var originCell = Board.FirstOrDefault(c => c.Row == instruction.InitialX && c.Column == instruction.InitialY);
            var destinationCell = Board.FirstOrDefault(c => c.Row == instruction.FinalX && c.Column == instruction.FinalY);

            if (originCell == null || destinationCell == null)
            {
                MessageBox.Show("No se pudieron encontrar las celdas relevantes para este movimiento.");
                return;
            }

            // Actualizamos tanto la celda de origen como la de destino independientemente del resultado.
            switch (instruction.Result)
            {
                case "Kill":
                    MessageBox.Show("¡Destruiste la pieza del oponente!");
                    UpdateCellState(originCell, null, false, null); // Origen vacío
                    UpdateCellState(destinationCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece); // Destino con nueva pieza
                    break;

                case "Fail":
                    MessageBox.Show("Tu movimiento no tuvo éxito.");
                    UpdateCellState(originCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece); // Origen no cambia
                    UpdateCellState(destinationCell, destinationCell.OccupiedPieceImage, true, destinationCell.OccupyingPiece); // Destino no cambia
                    break;

                case "Draw":
                    MessageBox.Show("¡Empate! Ninguna pieza fue destruida.");
                    UpdateCellState(originCell, null, false, null); // Ambos vacíos
                    UpdateCellState(destinationCell, null, false, null);
                    break;

                case "Move":
                    MessageBox.Show("¡Movimiento exitoso!");
                    UpdateCellState(originCell, null, false, null); // Origen vacío
                    UpdateCellState(destinationCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece); // Destino con nueva pieza
                    break;

                default:
                    MessageBox.Show($"Resultado desconocido: {instruction.Result}");
                    break;
            }

            OnPropertyChanged(nameof(Board));
        }

        private void UpdateCellState(Cell cell, BitmapImage pieceImage, bool isOccupied, Piece occupyingPiece)
        {
            cell.OccupiedPieceImage = pieceImage;
            cell.IsOccupied = isOccupied;
            cell.OccupyingPiece = occupyingPiece;

            OnPropertyChanged(nameof(cell.OccupiedPieceImage));
            OnPropertyChanged(nameof(cell.IsOccupied));
            OnPropertyChanged(nameof(cell.OccupyingPiece));
        }

    }
}
