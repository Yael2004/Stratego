﻿using StrategoApp.GameService;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using StrategoApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

        public readonly List<(int Row, int Column)> invalidPositions = new List<(int Row, int Column)> { };

        public ObservableCollection<Piece> AvailablePices { get; set; }


        public ICommand SendPositionCommand { get; }

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
            LoadPlayerData();
            InitializeBoard();
            InitializePlayerPieces();
        }

        public GameViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _gameServiceClient = new GameServiceClient(new System.ServiceModel.InstanceContext(this));
            _mainWindowViewModel = mainWindowViewModel;
            PlayerPieces = new ObservableCollection<Piece>();
            Board = new ObservableCollection<Cell>();
            LoadPlayerData();
            InitializeBoard();
            InitializePlayerPieces();
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

        public void TemporalSendPosition()
        {

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
                    Board.Add(new Cell { Row = row, Column = col });
                }
            }
        }

        private void InitializePlayerPieces()
        {
            for (int i = 0; i < 10; i++)
            {

                PlayerPieces.Add(new Piece
                {
                    Id = i + 1,
                    Name = $"Piece {i + 1}",
                    PieceImage = new BitmapImage(new Uri($"pack://application:,,,/StrategoApp;component/Assets/Game/Dragon.png"))
                });
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
                        var piece = AvailablePices.FirstOrDefault(p => p.Id == position.PieceId);
                        if (piece != null)
                        {
                            cell.OccupiedPieceImage = piece.PieceImage;
                            cell.IsOccupied = true;
                            cell.OccupyingPiece = piece;
                        }
                    }
                }
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
                PieceId = Board.FirstOrDefault(cell => cell.Row == targetRow && cell.Column == targetColumn)?.OccupyingPiece?.Id ?? 0,
                MoveType = "move"
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
            if (operationResult.IsSuccess)
            {
                MessageBox.Show(position.FinalX.ToString() + "\n" + position.FinalY.ToString());
            }
        }

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
    }
}
