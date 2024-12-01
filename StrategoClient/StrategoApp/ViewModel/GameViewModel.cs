using StrategoApp.GameService;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using StrategoApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
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
        private int _accountId;
        private string _profilePicture;
        private string _opponentUsername;
        private int _opponentId;
        private string _opponentProfilePicture;
        private int _selectedPieceId;
        private int _gameId;
        private bool _playerStartTurn;
        private bool _isMyTurn;
        private bool _isWonGame;
        private bool _isGameResultPopupOpen;
        private string _gameResultText;
        private Cell _selectedCell;

        private MainWindowViewModel _mainWindowViewModel;
        private GameServiceClient _gameServiceClient;
        private readonly OtherProfileDataServiceClient _otherProfileDataService;
        public ObservableCollection<Piece> PlayerPieces { get; set; }
        public ObservableCollection<Cell> Board { get; set; }
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
            _otherProfileDataService = new OtherProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));

            PlayerPieces = new ObservableCollection<Piece>();
            Board = new ObservableCollection<Cell>();
            CellClickedCommand = new ViewModelCommandGeneric<Cell>(OnCellClicked);
            LoadPlayerData();
            InitializeBoard();
        }

        public GameViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _gameServiceClient = new GameServiceClient(new System.ServiceModel.InstanceContext(this));
            _otherProfileDataService = new OtherProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));

            _mainWindowViewModel = mainWindowViewModel;
            PlayerPieces = new ObservableCollection<Piece>();
            Board = new ObservableCollection<Cell>();
            CellClickedCommand = new ViewModelCommandGeneric<Cell>(OnCellClicked);

            invalidPositions.Add((5, 3));
            invalidPositions.Add((5, 4));
            invalidPositions.Add((6, 3));
            invalidPositions.Add((6, 4));
            invalidPositions.Add((5, 7));
            invalidPositions.Add((5, 8));
            invalidPositions.Add((6, 7));
            invalidPositions.Add((6, 8));

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

        public int AccountId
        {
            get => _accountId;
            set
            {
                _accountId = value;
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

        public bool IsMyTurn
        {
            get => _isMyTurn;
            set
            {
                if (_isMyTurn != value)
                {
                    _isMyTurn = value;
                    OnPropertyChanged(nameof(IsMyTurn));
                }
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

        public bool IsGameResultPopupOpen
        {
            get => _isGameResultPopupOpen;
            set
            {
                _isGameResultPopupOpen = value;
                OnPropertyChanged(nameof(IsGameResultPopupOpen));
            }
        }

        public string GameResultText
        {
            get => _gameResultText;
            set
            {
                _gameResultText = value;
                OnPropertyChanged(nameof(GameResultText));
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
                AccountId = PlayerSingleton.Instance.Player.AccountId;
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
                            piece.PowerLevel = piece.PowerLevel;
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

        private bool isNecronomicon(Cell originCell, Cell destinationCell)
        {
            if (destinationCell.OccupyingPiece.Name == "Necronomicon")
            {
                return true;
            }

            return false;
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
            if (IsMyTurn)
            {
                return;
            }

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
            SendUpdatedPositionToServer(originCell.Row, originCell.Column, destinationCell.Row, destinationCell.Column, movingPiece.PowerLevel);
        }

        private void HandleTrapbreakerRule(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
                MessageBox.Show("¡El Trapbreaker desactivó una poción!");
                destinationCell.OccupiedPieceImage = originCell.OccupiedPieceImage;
                destinationCell.IsOccupied = true;
                destinationCell.OccupyingPiece = movingPiece;

                originCell.OccupiedPieceImage = null;
                originCell.IsOccupied = false;
                originCell.OccupyingPiece = null;

                //SendUpdatedPositionToServer(originCell.Row, originCell.Column, destinationCell.Row, destinationCell.Column);
        }

        private void HandleAbysswatcherRule(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            var defenderPiece = destinationCell.OccupyingPiece;

            if (movingPiece.Name == "Abysswatcher" && defenderPiece.Name == "Archfiend")
            {
                MessageBox.Show("¡El Abysswatcher eliminó al Archfiend!");
                destinationCell.OccupiedPieceImage = originCell.OccupiedPieceImage;
                destinationCell.IsOccupied = true;
                destinationCell.OccupyingPiece = movingPiece;

                originCell.OccupiedPieceImage = null;
                originCell.IsOccupied = false;
                originCell.OccupyingPiece = null;

                //SendUpdatedPositionToServer(originCell.Row, originCell.Column, destinationCell.Row, destinationCell.Column);
            }
        }


        public void SendUpdatedPositionToServer(int initialRow, int initialColumn, int targetRow, int targetColumn, int powerLevel)
        {
            var position = new PositionDTO
            {
                InitialX = initialRow,
                InitialY = initialColumn,
                FinalX = targetRow,
                FinalY = targetColumn,
                PowerLevel = powerLevel,
                MoveType = "move"
            };

            SendPositionToServer(position);
            IsMyTurn = true;
        }


        private async void SendPositionToServer(PositionDTO position)
        {
            try
            {
                await _gameServiceClient.SendPositionAsync(_gameId, UserId, position);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar la posición: {ex.Message}");
            }
        }

        public void LoadOponentPlayerInfo(string opponentUserName, string opponentProfilePicture)
        {
            OpponentUsername = opponentUserName;
            OpponentProfilePicture = opponentProfilePicture;
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                OpponentUsername = response.PlayerInfo.PlayerInfo.Name;
                OpponentId = response.PlayerInfo.PlayerInfo.Id;
                OpponentProfilePicture = response.PlayerInfo.PlayerInfo.PicturePath;
            }
        }

        public void OnGameStarted(int gameId, GameStartedResponse gameStartedResponse)
        {
            _gameId = gameId;
            if (gameStartedResponse.OperationResult.IsSuccess)
            {
                MessageBox.Show(gameStartedResponse.IsStarter.ToString());
                IsMyTurn = gameStartedResponse.IsStarter;
                _mainWindowViewModel.ChangeViewModel(new GameSetupViewModel(_mainWindowViewModel, this));
            }
            else
            {
                MessageBox.Show("Error starting game: " + gameStartedResponse.OperationResult.Message);
            }
        }

        public void OnReceiveOpponentPosition(PositionDTO position, GameService.OperationResult operationResult)
        {
            if (!operationResult.IsSuccess)
            {
                MessageBox.Show($"Error del servidor: {operationResult.Message}");
                return;
            }

            IsMyTurn = false;

            int invertedInitialRow = Math.Abs(9 - position.InitialX);
            int invertedFinalRow = Math.Abs(9 - position.FinalX);

            var originCell = Board.FirstOrDefault(c => c.Row == invertedInitialRow && c.Column == position.InitialY);
            var destinationCell = Board.FirstOrDefault(c => c.Row == invertedFinalRow && c.Column == position.FinalY);

            if (originCell == null || destinationCell == null)
            {
                MessageBox.Show("Error al procesar las celdas de origen o destino.");
                return;
            }

            string result = ProcessMove(originCell, destinationCell, position.PowerLevel);

            var movementInstruction = new MovementInstructionDTO
            {
                DefenderId = UserId,
                InitialX = position.InitialX,
                InitialY = position.InitialY,
                FinalX = position.FinalX,
                FinalY = position.FinalY,
                Result = result
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

        public void ShowGameResult(bool isWinner)
        {
            string resultMessage = isWinner ? "Victory" : "Defeat";
            IsGameResultPopupOpen = true;

            MessageBox.Show(resultMessage);

            GoToLobby();
        }

        private async void EndGame()
        {
            try
            {
                await _gameServiceClient.EndGameAsync(_gameId, AccountId, _isWonGame);
                ShowGameResult(_isWonGame);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al finalizar el juego: {ex.Message}");
            }
        }

        private void GoToLobby()
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        public async void SendMyMovement(MovementInstructionDTO movementInstruction)
        {
            await _gameServiceClient.SendMovementInstructionsAsync(_gameId, movementInstruction);
        }

        private string ProcessMove(Cell originCell, Cell destinationCell, int oponnentPowerLevel)
        {
            var defenderPiece = destinationCell.OccupyingPiece;

            if (destinationCell.IsOccupied && defenderPiece != null && defenderPiece.OwnerId == UserId)
            {
                if (oponnentPowerLevel > defenderPiece.PowerLevel)
                {
                    if (defenderPiece.Name == "Necronomicon")
                    {
                        _isWonGame = false;
                        UpdateCellState(destinationCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece);
                        UpdateCellState(originCell, null, false, null);
                        Task.Run(() =>EndGame());
                        return "Kill";
                    }
                    else
                    {
                        UpdateCellState(destinationCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece);
                        UpdateCellState(originCell, null, false, null);
                        return "Kill";
                    }
                }
                else if (oponnentPowerLevel < defenderPiece.PowerLevel)
                {
                    UpdateCellState(originCell, null, false, null);
                    return "Fail";
                }
                else
                {
                    UpdateCellState(destinationCell, null, false, null);
                    UpdateCellState(originCell, null, false, null);
                    return "Draw";
                }
            }
            else
            {
                UpdateCellState(destinationCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece);
                UpdateCellState(originCell, null, false, null);
                return "Move";
            }
        }

        private void UpdateCellState(Cell cell, BitmapImage pieceImage, bool isOccupied, Piece occupyingPiece)
        {
            cell.OccupiedPieceImage = pieceImage;
            cell.IsOccupied = isOccupied;
            cell.OccupyingPiece = occupyingPiece;

            OnPropertyChanged(nameof(Board));
        }

        public void OnOpponentAbandonedGame(GameService.OperationResult operationResult)
        {
            throw new NotImplementedException();
        }

        public void OnGameEnded(string resultString, GameService.OperationResult operationResult)
        {
            if (operationResult.IsSuccess)
            {
                MessageBox.Show(resultString);
            }
            else
            {
                MessageBox.Show("Error al finalizar el juego: " + operationResult.Message);
            }
        }

        public async void SuscribeToGame(int gameId)
        {
            await _gameServiceClient.JoinGameSessionAsync(gameId, UserId);
        }

        public async void GetOtherPlayerInfo(int opponentId)
        {
            if (opponentId > 0)
            {
                await _otherProfileDataService.GetOtherPlayerInfoAsync(opponentId, UserId);
            }
            else
            {
                OpponentId = opponentId;
                OpponentUsername = "Invited";
                OpponentProfilePicture = "pack://application:,,,/StrategoApp;component/Assets/Images/ProfilePictures/Picture1.png";
            }
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

            var pieceImage = originCell.OccupiedPieceImage;
            var occupyingPiece = originCell.OccupyingPiece;

            switch (instruction.Result)
            {
                case "Kill":
                    if (destinationCell.OccupyingPiece.Name == "Necronomicon")
                    {
                        _isWonGame = true;
                        UpdateCellState(destinationCell, pieceImage, true, occupyingPiece);
                        UpdateCellState(originCell, null, false, null);
                        Task.Run(() => EndGame());
                        break;
                    }
                    else
                    {
                        UpdateCellState(destinationCell, pieceImage, true, occupyingPiece);
                        UpdateCellState(originCell, null, false, null);
                        break;
                    }

                case "Fail":
                    UpdateCellState(originCell, null, false, null);
                    break;

                case "Draw":
                    UpdateCellState(destinationCell, null, false, null);
                    UpdateCellState(originCell, null, false, null);
                    break;

                case "Move":
                    UpdateCellState(destinationCell, pieceImage, true, occupyingPiece);
                    UpdateCellState(originCell, null, false, null);
                    break;

                default:
                    MessageBox.Show($"Resultado desconocido: {instruction.Result}");
                    break;
            }

            OnPropertyChanged(nameof(Board));
        }
    }
}
