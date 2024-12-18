﻿using log4net;
using StrategoApp.GameService;
using StrategoApp.Helpers;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using StrategoApp.RoomService;
using StrategoApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class RoomViewModel : ViewModelBase, RoomService.IRoomServiceCallback, ProfileService.IOtherProfileDataServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _username;
        private string _profilePicture;
        private string _usernameOponent;
        private string _profilePictureOponent;
        private string _messageToSend;
        private string _reportedResultMessage;
        private string _reportMessage;
        private string _exceptionMessage;
        private int _userId;
        private int _userIdOponent;
        private int _gameId;
        private int _reportsNumber;
        private bool _isReportVisible;
        private bool _isReportButtonVisible;
        private bool _isPlayAvalible;
        private bool _isReportedMessageVisible;
        private bool _isServiceErrorVisible;

        private ObservableCollection<string> _messages;
        
        private readonly RoomServiceClient _roomServiceClient;
        private readonly OtherProfileDataServiceClient _otherProfileDataService;
        private readonly CreateGameServiceClient _gameServiceClient;
        
        private readonly GameViewModel _gameViewModel;
        private readonly PingCheck _pingCheck;


        private readonly MainWindowViewModel _mainWindowViewModel;
        public ICommand ExecuteBackToLobbyCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand ToggleReportVisibilityCommand { get; }
        public ICommand SendReportCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand SetReportMessageCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }

        public string RoomCode { get; set; }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string ProfilePicture
        {
            get { return _profilePicture; }
            set
            {
                _profilePicture = value;
                OnPropertyChanged();
            }
        }

        public string UsernameOponent
        {
            get { return _usernameOponent; }
            set
            {
                _usernameOponent = value;
                OnPropertyChanged();
            }
        }

        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set
            {
                _exceptionMessage = value;
                OnPropertyChanged();
            }
        }

        public int UserIdOponent
        {
            get { return _userIdOponent; }
            set
            {
                _userIdOponent = value;
                OnPropertyChanged();
            }
        }

        public int ReportsNumber
        {
            get { return _reportsNumber; }
            set
            {
                _reportsNumber = value;
                OnPropertyChanged();
            }
        }

        public string ProfilePictureOponent
        {
            get { return _profilePictureOponent; }
            set
            {
                _profilePictureOponent = value;
                OnPropertyChanged();
            }
        }

        public string MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                _messageToSend = value;
                OnPropertyChanged();
            }
        }

        public string ReportedResultMessage
        {
            get { return _reportedResultMessage; }
            set
            {
                _reportedResultMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsReportVisible
        {
            get { return _isReportVisible; }
            set
            {
                _isReportVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsReportButtonVisible
        {
            get { return _isReportButtonVisible; }
            set
            {
                _isReportButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsPlayAvalible
        {
            get { return _isPlayAvalible; }
            set
            {
                _isPlayAvalible = value;
                OnPropertyChanged();
            }
        }

        public bool IsReportedMessageVisible
        {
            get { return _isReportedMessageVisible; }
            set
            {
                _isReportedMessageVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsServiceErrorVisible
        {
            get { return _isServiceErrorVisible; }
            set
            {
                _isServiceErrorVisible = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged(nameof(Messages));
                }
            }
        }

        public RoomViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _roomServiceClient = new RoomServiceClient(new InstanceContext(this));
            _otherProfileDataService = new OtherProfileDataServiceClient(new InstanceContext(this));
            _gameServiceClient = new CreateGameServiceClient();

            _mainWindowViewModel = mainWindowViewModel;

            _pingCheck = new PingCheck(_mainWindowViewModel);
            Task.Run(() => _pingCheck.StartPingMonitoringAsync());

            ExecuteBackToLobbyCommand = new ViewModelCommand(ExecuteBackToLobby);
            SendMessageCommand = new ViewModelCommand(SendMessageAsync);
            ToggleReportVisibilityCommand = new ViewModelCommand(ToggleReportVisibility);
            SendReportCommand = new ViewModelCommand(SendReport);
            PlayCommand = new ViewModelCommand(ExecutePlay, CanExecutePlay);
            SetReportMessageCommand = new ViewModelCommand(SetReportMessage);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(CloseServiceError);

            _messages = new ObservableCollection<string>();
            _gameViewModel = new GameViewModel(_mainWindowViewModel);
            IsReportVisible = false;
            ReportsNumber = 0;

            LoadPlayerData();
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

        public async Task<bool> CreateARoomAsync()
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;

                await _roomServiceClient.CreateRoomAsync(playerId);
                
                return true;
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while creating room.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();

                return false;
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while creating room.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();

                return false;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while creating room.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();

                return false;
            }
        }

        public async Task<bool> JoinToRoomAsync(string roomCode)
        {
            bool canJoin = false;
            var playerId = PlayerSingleton.Instance.Player.Id;
            RoomCode = roomCode;

            try
            {
                canJoin = await _roomServiceClient.JoinRoomAsync(RoomCode, playerId);

                if (canJoin)
                {
                    await Task.Run(() => _roomServiceClient.NotifyPlayersOfNewConnectionAsync(RoomCode, UserId));
                    IsPlayAvalible = true;
                }
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while joining to room.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while joining to room.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while joining to room.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }

            return canJoin;
        }

        public void SendMessageAsync(object obj)
        {

            if (MessageToSend != string.Empty && MessageToSend != null)
            {
                MessageToSend = MessageToSend.Trim();
                
                try
                {
                    var playerId = PlayerSingleton.Instance.Player.Id;

                    _roomServiceClient.SendMessageToRoomAsync(RoomCode, playerId, MessageToSend);

                    MessageToSend = string.Empty;
                }
                catch (CommunicationException cex)
                {
                    Log.Error("Communication error while sending message.", cex);
                    ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                    IsServiceErrorVisible = true;
                    _pingCheck.StopPingMonitoring();
                }
                catch (TimeoutException tex)
                {
                    Log.Error("Timed out while sending message.", tex);
                    ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                    IsServiceErrorVisible = true;
                    _pingCheck.StopPingMonitoring();
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while sending message.", ex);
                    ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                    IsServiceErrorVisible = true;
                    _pingCheck.StopPingMonitoring();
                }
            }
        }

        public void ToggleReportVisibility(object obj)
        {
            _reportMessage = string.Empty;
            IsReportVisible = !IsReportVisible;
        }

        public void SendReport(object obj)
        {
            if (_reportMessage != string.Empty)
            {
                ReportPlayer();
                _reportMessage = string.Empty;
            }
        }

        private async void ReportPlayer()
        {
            try
            {
                await Task.Run(() => _roomServiceClient.ReportPlayerAccountAsync(UserId, UserIdOponent, _reportMessage));
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while reporting player.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while reporting player.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while reporting player.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
        }

        public void SetReportMessage(object obj)
        {
            _reportMessage = obj.ToString();
        }

        public void ExecutePlay(object obj)
        {
            CreateGameCode();
        }

        private async void CreateGameCode()
        {
            try
            {
                var response = await _gameServiceClient.CreateGameSessionAsync();

                if (response.OperationResult.IsSuccess)
                {
                    _gameId = response.GameId;

                    NotifyOpponentToJoinGame();
                }
                else if (response.OperationResult.IsDataBaseError)
                {
                    ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                    IsServiceErrorVisible = true;
                }
                else
                {
                    Log.Warn($"Failed to create game session: {response.OperationResult.Message}");
                }

            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while obtaining verification code.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while getting verification code.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while getting verification code.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
        }

        private async void NotifyOpponentToJoinGame()
        {
            try
            {
                await Task.Run(() => _roomServiceClient.NotifyOpponentToJoinGameAsync(RoomCode, _gameId));
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while notifying opponent to join game.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while notifying opponent to join game.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while notifying opponent to join game.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
        }

        public bool CanExecutePlay(object obj)
        {
            if (IsPlayAvalible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ExecuteBackToLobby(Object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            _pingCheck.StopPingMonitoring();
        }

        public void RoomCreatedAsync(RoomCreatedResponse response)
        {
            if (response.Result.IsSuccess)
            {
                RoomCode = response.RoomCode;
            }
            else if (response.Result.IsDataBaseError)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
            else
            {
                Log.Warn($"Failed to create a room {response.Result.Message}");            
            }
        }

        public void RoomResponseAsync(RoomService.OperationResult response)
        {
            if (response.IsSuccess)
            {
                Log.Info($"Room response: {response.Message}");
            }
            if (response.IsDataBaseError)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
            else
            {
                Log.Warn($"Room response: {response.Message}");
            }
        }

        public void ReceiveMessageAsync(int playerId, string message)
        {
            Messages.Add($"{playerId} {message}");
            OnPropertyChanged(nameof(Messages));
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                UsernameOponent = response.PlayerInfo.PlayerInfo.Name;
                UserIdOponent = response.PlayerInfo.PlayerInfo.Id;
                ProfilePictureOponent = response.PlayerInfo.PlayerInfo.PicturePath;

                if (ReportsNumber == 0)
                {
                    IsReportButtonVisible = true;
                }
                else
                {
                    IsReportButtonVisible = false;
                }
            }
            else if (response.Result.IsSuccess)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
            else
            {
                Log.Warn($"Failed to get other player info: {response.Result.Message}");
            }
        }

        public void GetConnectedPlayerId(int connectedPlayerId)
        {
            if (connectedPlayerId > 0)
            {
                UserIdOponent = connectedPlayerId;
                CallOponentPlayerInfo(connectedPlayerId);
            }
            else
            {
                UserIdOponent = connectedPlayerId;
                UsernameOponent = "Guest";
                ProfilePictureOponent = "pack://application:,,,/StrategoApp;component/Assets/Images/ProfilePictures/Picture1.png";
            }
        }

        private void CallOponentPlayerInfo(int connectedPlayerId)
        {
            _otherProfileDataService.GetOtherPlayerInfoAsync(connectedPlayerId, UserId);
        }

        public void NotifyToJoinGame(int gameId, RoomService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                _gameId = gameId;
                _gameViewModel.SuscribeToGame(gameId);

                _pingCheck.StopPingMonitoring();

                _gameViewModel.GetOtherPlayerInfo(UserIdOponent);
            }
            else if (result.IsDataBaseError)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
            else
            {
                Log.Warn($"Failed to notify to join game: {result.Message}");
            }
        }

        public void NotifyPlayerReported(RoomService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                IsReportedMessageVisible = true;
                ReportsNumber = 1;
            }
            else if (result.IsDataBaseError)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
            else
            {
                Log.Warn($"Failed to report player: {result.Message}");
            }
        }

        private void CloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
        }

    }
}
