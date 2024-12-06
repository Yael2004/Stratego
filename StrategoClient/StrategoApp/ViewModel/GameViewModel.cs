using log4net;
using StrategoApp.GameService;
using StrategoApp.Helpers;
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
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _username;
        private string _profilePicture;
        private string _opponentUsername;
        private string _opponentProfilePicture;
        private string _gameResultText;
        private bool _isMyTurn;
        private bool _isWonGame;
        private bool _isGameResultPopupOpen;
        private bool _isServiceErrorVisible;
        private int _userId;
        private int _accountId;
        private int _opponentId;
        private int _gameId;
        private Cell _selectedCell;

        private readonly GameServiceClient _gameServiceClient;
        private readonly OtherProfileDataServiceClient _otherProfileDataService;

        private readonly MainWindowViewModel _mainWindowViewModel;
        public ObservableCollection<Piece> PlayerPieces { get; set; }
        public ObservableCollection<Cell> Board { get; set; }

        private readonly List<(int Row, int Column)> invalidPositions = new List<(int Row, int Column)> { };

        public ObservableCollection<Piece> AvailablePices { get; set; }

        public ICommand CellClickedCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }

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

        public bool IsServiceErrorVisible
        {
            get => _isServiceErrorVisible;
            set
            {
                _isServiceErrorVisible = value;
                OnPropertyChanged(nameof(IsServiceErrorVisible));
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

        public GameViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _gameServiceClient = new GameServiceClient(new InstanceContext(this));
            _otherProfileDataService = new OtherProfileDataServiceClient(new InstanceContext(this));

            _mainWindowViewModel = mainWindowViewModel;

            CellClickedCommand = new ViewModelCommandGeneric<Cell>(OnCellClicked);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(CloseServiceError);

            PlayerPieces = new ObservableCollection<Piece>();
            Board = new ObservableCollection<Cell>();


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

            IsServiceErrorVisible = false;
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
                        cell.IsOccupied = true;
                        cell.OccupiedPieceImage = new BitmapImage(new Uri($"pack://application:,,,/StrategoApp;component/Assets/Game/Dragon.png"));
                        cell.OccupyingPiece = new Piece
                        {
                            Color = "Red"
                        };
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
                        }
                    }
                }
            }
        }

        private void CloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        public bool IsValidMove(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            if (invalidPositions.Contains((destinationCell.Row, destinationCell.Column)))
            {
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
                SelectedCell = null;
                return false;
            }

            return isAdjacent;
        }

        public bool IsPathClear(Cell originCell, Cell destinationCell)
        {
            if (originCell.Row == destinationCell.Row)
            {
                return IsPathClearInRow(originCell, destinationCell);
            }
            else if (originCell.Column == destinationCell.Column)
            {
                return IsPathClearInColumn(originCell, destinationCell);
            }

            return false;
        }

        private bool IsPathClearInRow(Cell originCell, Cell destinationCell)
        {
            int minColumn = Math.Min(originCell.Column, destinationCell.Column);
            int maxColumn = Math.Max(originCell.Column, destinationCell.Column);
            for (int col = minColumn + 1; col < maxColumn; col++)
            {
                var cell = Board.FirstOrDefault(c => c.Row == originCell.Row && c.Column == col);
                if (cell != null && cell.IsOccupied)
                    return false;
            }
            return true;
        }

        private bool IsPathClearInColumn(Cell originCell, Cell destinationCell)
        {
            int minRow = Math.Min(originCell.Row, destinationCell.Row);
            int maxRow = Math.Max(originCell.Row, destinationCell.Row);
            for (int row = minRow + 1; row < maxRow; row++)
            {
                var cell = Board.FirstOrDefault(c => c.Row == row && c.Column == originCell.Column);
                if (cell != null && cell.IsOccupied)
                    return false;
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

                bool isValidMove = IsValidMove(SelectedCell, clickedCell, movingPiece);

                if (isValidMove)
                {
                    HandleMove(SelectedCell, clickedCell, movingPiece);
                }

                SelectedCell = null;
            }
        }


        private void HandleMove(Cell originCell, Cell destinationCell, Piece movingPiece)
        {
            var position = new PositionDTO
            {
                InitialX = originCell.Row,
                InitialY = originCell.Column,
                FinalX = destinationCell.Row,
                FinalY = destinationCell.Column,
                PieceName = movingPiece.Name,
                PowerLevel = movingPiece.PowerLevel,
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
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while sending position to server.", cex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while sending position to server.", tex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while sending position to server.", ex);
                IsServiceErrorVisible = true;
            }
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
                IsMyTurn = gameStartedResponse.IsStarter;
                _mainWindowViewModel.ChangeViewModel(new GameSetupViewModel(_mainWindowViewModel, this));
            }
            else
            {
                Log.Warn("Error starting game: " + gameStartedResponse.OperationResult.Message);
            }
        }

        public void OnReceiveOpponentPosition(PositionDTO position, GameService.OperationResult operationResult)
        {
            if (!operationResult.IsSuccess)
            {
                Log.Warn($"Error on recieve opponent position: {operationResult.Message}");
                return;
            }

            IsMyTurn = false;

            int invertedInitialRow = Math.Abs(9 - position.InitialX);
            int invertedFinalRow = Math.Abs(9 - position.FinalX);

            var originCell = Board.FirstOrDefault(c => c.Row == invertedInitialRow && c.Column == position.InitialY);
            var destinationCell = Board.FirstOrDefault(c => c.Row == invertedFinalRow && c.Column == position.FinalY);

            if (originCell == null || destinationCell == null)
            {
                Log.Warn($"Error processing source or destination cells.");
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

            Task.Run(async () =>
            {
                try
                {
                    await _gameServiceClient.SendMovementInstructionsAsync(_gameId, movementInstruction);
                    if (result == "Win")
                    {
                        await Task.Run(() => EndGame());
                    }

                }
                catch (CommunicationException cex)
                {
                    Log.Error("Communication error while sending movment instructions.", cex);
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error("Timed out while sending movment instructions.", tex);
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while sending movment instructions.", ex);
                    IsServiceErrorVisible = true;
                }
            });
        }

        public void ShowGameResult(bool isWinner)
        {
            GameResultText = isWinner ? "Victory!" : "Defeat!";
            IsGameResultPopupOpen = true;

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                Application.Current.Dispatcher.Invoke(GoToLobby);
            });
        }

        private async void EndGame()
        {
            try
            {
                var finalStats = new FinalStatsDTO
                {
                    GameId = _gameId,
                    AccountId = AccountId,
                    PlayerId = UserId,
                    HasWon = _isWonGame
                };

                await _gameServiceClient.EndGameAsync(finalStats);
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while ending game.", cex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while ending game.", tex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while ending game.", ex);
                IsServiceErrorVisible = true;
            }
        }

        private void GoToLobby()
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        private string ProcessMove(Cell originCell, Cell destinationCell, int oponnentPowerLevel)
        {
            var defenderPiece = destinationCell.OccupyingPiece;

            if (destinationCell.IsOccupied && defenderPiece != null && defenderPiece.OwnerId == UserId)
            {
                if (oponnentPowerLevel > defenderPiece.PowerLevel)
                {
                    if (defenderPiece.PowerLevel == -2)
                    {
                        _isWonGame = false;
                        UpdateCellState(destinationCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece);
                        UpdateCellState(originCell, null, false, null);

                        return "Win";
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
            if (operationResult.IsSuccess)
            {
                MessageBox.Show("El oponente abandonó el juego.");
                GoToLobby();
            }
            else
            {
                MessageBox.Show("Error al abandonar el juego: " + operationResult.Message);
            }
        }

        public void OnGameEnded(string resultString, GameService.OperationResult operationResult)
        {
            if (operationResult.IsSuccess)
            {
                ShowGameResult(_isWonGame);
            }
            else
            {
                MessageBox.Show("Error al finalizar el juego: " + operationResult.Message);
            }
        }

        public async void SuscribeToGame(int gameId)
        {
            try
            {
                await _gameServiceClient.JoinGameSessionAsync(gameId, UserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al unirse al juego: {ex.Message}");
            }
        }

        public async void GetOtherPlayerInfo(int opponentId)
        {
            if (opponentId > 0)
            {
                try
                {
                    await _otherProfileDataService.GetOtherPlayerInfoAsync(opponentId, UserId);
                }
                catch (CommunicationException cex)
                {
                    Log.Error("Communication error with the connect service.", cex);
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error("Timed out while communicating with the connect service.", tex);
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while connecting in.", ex);
                    IsServiceErrorVisible = true;
                }
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
            if (!IsMovementInstructionValid(movementInstructionResponse))
                return;

            var instruction = movementInstructionResponse.MovementInstructionDTO;

            if (instruction == null)
            {
                Log.Error($"The move instruction data is null. {movementInstructionResponse.OperationResult.Message}");
                return;
            }

            var (originCell, destinationCell) = GetCells(instruction);
            if (originCell == null || destinationCell == null)
            {
                Log.Error($"The relevant cells for this move could not be found: {movementInstructionResponse.OperationResult.Message}");
                return;
            }

            ProcessMovementInstruction(instruction, originCell, destinationCell);
            OnPropertyChanged(nameof(Board));
        }

        private (Cell originCell, Cell destinationCell) GetCells(MovementInstructionDTO instruction)
        {
            var originCell = Board.FirstOrDefault(c => c.Row == instruction.InitialX && c.Column == instruction.InitialY);
            var destinationCell = Board.FirstOrDefault(c => c.Row == instruction.FinalX && c.Column == instruction.FinalY);
            return (originCell, destinationCell);
        }

        private void ProcessMovementInstruction(MovementInstructionDTO instruction, Cell originCell, Cell destinationCell)
        {
            switch (instruction.Result)
            {
                case "Win":
                    HandleWin(destinationCell, originCell);
                    break;

                case "Kill":
                    HandleKill(destinationCell, originCell);
                    break;

                case "Fail":
                    HandleFail(originCell);
                    break;

                case "Draw":
                    HandleDraw(destinationCell, originCell);
                    break;

                case "Move":
                    HandleMove(destinationCell, originCell);
                    break;

                default:
                    Log.Warn($"Unknown Result: {instruction.Result}");
                    break;
            }
        }

        private bool IsMovementInstructionValid(MovementInstructionResponse movementInstructionResponse)
        {
            if (movementInstructionResponse == null || movementInstructionResponse.OperationResult == null)
            {
                Log.Error($"The movement response is null or invalid: {movementInstructionResponse.OperationResult.Message}");
                return false;
            }

            if (!movementInstructionResponse.OperationResult.IsSuccess)
            {
                Log.Error($"Movement error: {movementInstructionResponse.OperationResult.Message}");
                return false;
            }

            return true;
        }

        private void HandleWin(Cell destinationCell, Cell originCell)
        {
            _isWonGame = true;
            UpdateCellKillMove(destinationCell, originCell);
            Task.Run(() => EndGame());
        }

        private void HandleKill(Cell destinationCell, Cell originCell)
        {
            UpdateCellKillMove(destinationCell, originCell);
        }

        private void HandleFail(Cell originCell)
        {
            UpdateCellState(originCell, null, false, null);
        }

        private void HandleDraw(Cell destinationCell, Cell originCell)
        {
            UpdateCellDraw(destinationCell, originCell);
        }

        private void HandleMove(Cell destinationCell, Cell originCell)
        {
            UpdateCellKillMove(destinationCell, originCell);
        }

        private void UpdateCellKillMove(Cell destinationCell, Cell originCell)
        {
            UpdateCellState(destinationCell, originCell.OccupiedPieceImage, true, originCell.OccupyingPiece);
            UpdateCellState(originCell, null, false, null);
        }

        private void UpdateCellDraw(Cell destinationCell, Cell originCell)
        {
            UpdateCellState(destinationCell, null, false, null);
            UpdateCellState(originCell, null, false, null);
        }
    }
}
