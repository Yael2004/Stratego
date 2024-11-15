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
        private RoomServiceClient _roomServiceClient;
        private OtherProfileDataServiceClient _otherProfileDataService;

        private string _username;
        private int _userId;
        private string _profilePicture;
        private string _usernameOponent;
        private int _userIdOponent;
        private string _profilePictureOponent;
        private string _messageToSend;
        private bool _isReportVisible;
        private bool _isPlayAvalible;

        private ObservableCollection<string> _messages;

        private readonly MainWindowViewModel _mainWindowViewModel;
        public ICommand ExecuteBackToLobbyCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand ToggleReportVisibilityCommand { get; }
        public ICommand CancelReportCommand { get; }
        public ICommand PlayCommand { get; }

        public string RoomCode { get; set; }

        public RoomViewModel(MainWindowViewModel mainWindowViewModel)
        {
            ExecuteBackToLobbyCommand = new ViewModelCommand(ExecuteBackToLobby);
            SendMessageCommand = new ViewModelCommand(SendMessageAsync);
            ToggleReportVisibilityCommand = new ViewModelCommand(ToggleReportVisibility);
            CancelReportCommand = new ViewModelCommand(CancelReport);
            PlayCommand = new ViewModelCommand(ExecutePlay, CanExecutePlay);

            _messages = new ObservableCollection<string>();
            _mainWindowViewModel = mainWindowViewModel;
            IsReportVisible = false;

            InitializeService();
            LoadPlayerData();
        }

        public RoomViewModel() { }

        private void InitializeService()
        {
            try
            {
                InstanceContext context = new InstanceContext(this);
                _roomServiceClient = new RoomServiceClient(context);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing the service client: " + ex.Message);
            }
        }

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

        public int UserIdOponent
        {
            get { return _userIdOponent; }
            set
            {
                _userIdOponent = value;
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

        public bool IsReportVisible
        {
            get { return _isReportVisible; }
            set
            {
                _isReportVisible = value;
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
            catch (Exception ex)
            {
                MessageBox.Show("Error creating room: " + ex.Message);
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
                    await _roomServiceClient.NotifyPlayersOfNewConnectionAsync(RoomCode, UserId);
                    IsPlayAvalible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error joining room: " + ex.Message);
            }

            return canJoin;
        }

        private void LeaveTheRoomAsync()
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                _roomServiceClient.LeaveRoomAsync(playerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error leaving room: " + ex.Message);
            }
        }

        public void SendMessageAsync(object obj)
        {
            if (string.IsNullOrWhiteSpace(MessageToSend))
            {
                MessageBox.Show("Cannot send an empty message.");
                return;
            }

            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                _roomServiceClient.SendMessageToRoomAsync(RoomCode, playerId, MessageToSend);
                MessageToSend = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending message: " + ex.Message);
            }
        }

        public void ToggleReportVisibility(object obj)
        {
            IsReportVisible = !IsReportVisible;
        }

        public void CancelReport(object obj)
        {
            IsReportVisible = false;
        }

        public void ExecutePlay(object obj)
        {
            try
            {
                var gameViewModel = new GameViewModel(_mainWindowViewModel);
                gameViewModel.SubscribeToGame(UserIdOponent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            try
            {
                _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void RoomCreatedAsync(RoomCreatedResponse response)
        {
            if (response.Result.IsSuccess)
            {
                RoomCode = response.RoomCode;
                MessageBox.Show("Room created successfully with code: " + RoomCode);
            }
            else
            {
                MessageBox.Show("Room creation failed: " + response.Result.Message);
            }
        }

        public void RoomResponseAsync(RoomService.OperationResult response)
        {
            if (response.IsSuccess)
            {
                MessageBox.Show(response.Message);
            }
            else
            {
                MessageBox.Show("Operation failed: " + response.Message);
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
            }
            else
            {
                MessageBox.Show($"{response.Result.Message}");
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
            _otherProfileDataService = new OtherProfileDataServiceClient(new InstanceContext(this));
            _otherProfileDataService.GetOtherPlayerInfoAsync(connectedPlayerId, UserId);
        }
    }
}
