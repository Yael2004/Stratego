using log4net;
using StrategoApp.FriendService;
using StrategoApp.Helpers;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using StrategoApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StrategoApp.ViewModel
{
    public class LobbyViewModel : ViewModelBase, Service.IChatServiceCallback, ProfileService.IPlayerFriendsListServiceCallback,
        ProfileService.IOtherProfileDataServiceCallback, FriendService.ISendRoomInvitationServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _messageToSend;
        private string _errorMessage;
        private string _username;
        private string _profilePicture;
        private string _joinRoomCode;
        private int _userId;
        private bool _isViewEnabled;
        private bool _isConnected;
        private bool _isJoinRoomDialogVisible;
        private bool _isServiceErrorVisible;

        private readonly ChatServiceClient _chatClient;
        private readonly OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private readonly PlayerFriendsListServiceClient _playerFriendsListServiceClient;
        private readonly SendRoomInvitationServiceClient _friendServiceClient;

        private readonly MainWindowViewModel _mainWindowViewModel;

        private RoomViewModel roomViewModel;
        private ObservableCollection<string> _messages;
        private ObservableCollection<Player> _friends;

        public ICommand SendMessagesCommand { get; }
        public ICommand ShowProfileCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand JoinToRoomCommand { get; }
        public ICommand CancelJoinToRoomCommand { get; }
        public ICommand CreateRoomCommand { get; }
        public ICommand JoinToRoomShowCommand { get; }
        public ICommand ShowScoreboardCommand { get; }
        public ICommand ShowFriendsCommand { get; }
        public ICommand InviteFriendCommand { get; }
        public ICommand CloseServiceErrorCommand { get; }

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

        public ObservableCollection<Player> Friends
        {
            get => _friends;
            set
            {
                _friends = value;
                OnPropertyChanged(nameof(Friends));
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

        public string JoinRoomCode
        {
            get => _joinRoomCode;
            set
            {
                _joinRoomCode = value;
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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsViewEnabled
        {
            get { return _isViewEnabled; }
            set
            {
                _isViewEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsJoinRoomDialogVisible
        {
            get => _isJoinRoomDialogVisible;
            set
            {
                _isJoinRoomDialogVisible = value;
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

        public LobbyViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _chatClient = new ChatServiceClient(new InstanceContext(this));
            _playerFriendsListServiceClient = new PlayerFriendsListServiceClient(new InstanceContext(this));
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _friendServiceClient = new SendRoomInvitationServiceClient(new InstanceContext(this));

            AssignValuesToUser();
            ConnectPlayerToChat();

            _mainWindowViewModel = mainWindowViewModel;

            ShowFriendsCommand = new ViewModelCommand(ShowFriends);
            SendMessageCommand = new ViewModelCommand(ClientSendMessage);
            ShowProfileCommand = new ViewModelCommand(ClientShowProfile);
            JoinToRoomShowCommand = new ViewModelCommand(JoinToRoomShow);
            JoinToRoomCommand = new ViewModelCommand(JoinToRoom);
            CancelJoinToRoomCommand = new ViewModelCommand(CancelJoinToRoom);
            CreateRoomCommand = new ViewModelCommand(CreateRoom);
            ShowScoreboardCommand = new ViewModelCommand(ShowScoreboard);
            InviteFriendCommand = new ViewModelCommand(InviteFriend);
            CloseServiceErrorCommand = new ViewModelCommand(ExecuteCloseServiceError);

            _messages = new ObservableCollection<string>();
            _friends = new ObservableCollection<Player>();

            LoadFriendsListAsync();

            IsConnected = false;
            _isJoinRoomDialogVisible = false;
        }

        public void ConnectPlayerToChat()
        {
            if (!_isConnected)
            {
                try
                {
                    _userId = _chatClient.ConnectAsync(_userId, _username).Result;

                    PlayerSingleton.Instance.Player.Id = _userId;
                }
                catch (CommunicationException ex)
                {
                    Log.Error("Communication error with the connect service.", ex);
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException ex)
                {
                    Log.Error("Timed out while communicating with the connect service.", ex);
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while connecting in.", ex);
                    IsServiceErrorVisible = true;
                }
            }
        }

        public void DisconnectPlayerFromChat()
        {
            if (_chatClient != null)
            {
                try
                {
                    _chatClient.Disconnect(_userId);
                    _chatClient.Close();
                }
                catch (CommunicationException ex)
                {
                    Log.Error("Communication error while disconnecting player from chat password.", ex);
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException ex)
                {
                    Log.Error("Timed out while disconnecting player from chat password.", ex);
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while disconnecting player from chat password.", ex);
                    IsServiceErrorVisible = true;
                }
            }
        }

        private void InviteFriend(object parameter)
        {
            if (parameter is Player friend)
            {
                SendFriendInvitation(friend);
            }
        }

        private async void SendFriendInvitation(Player friend)
        {
            try
            {
                roomViewModel = new RoomViewModel(_mainWindowViewModel);

                if (await roomViewModel.CreateARoomAsync())
                {
                    await _friendServiceClient.SendRoomInvitationAsync(friend.AccountId, roomViewModel.RoomCode);

                    DisconnectPlayerFromChat();
                    _mainWindowViewModel.ChangeViewModel(roomViewModel);
                }
                else
                {
                    Log.Warn("An error was ocurred on lobby send room invitation response.");
                }
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while sending invitation.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with sending invitation.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while sending invitation.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void AssignValuesToUser()
        {
            if (PlayerSingleton.Instance.IsLoggedIn())
            {
                var player = PlayerSingleton.Instance.Player;
                Username = player.Name;
                UserId = player.Id;
                ProfilePicture = player.PicturePath;
            }
        }

        public void ClientSendMessage(object obj)
        {
            try
            {
                _chatClient.SendMessage(_userId, _username, MessageToSend);
                MessageToSend = string.Empty;
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while sending message.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with sending message.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while sending message.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void ClientShowProfile(object obj)
        {
            try
            {
                DisconnectPlayerFromChat();
                _mainWindowViewModel.ChangeViewModel(new PlayerProfileViewModel(_mainWindowViewModel));
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while showing profile.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with showing profile.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while while showing profile.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void ShowScoreboard(object obj)
        {
            try
            {
                DisconnectPlayerFromChat();
                _mainWindowViewModel.ChangeViewModel(new ScoreboardViewModel(_mainWindowViewModel));
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while showing scoreboard.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with showing scoreboard.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while showing scoreboard.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void ShowFriends(object obj)
        {
            try
            {
                DisconnectPlayerFromChat();
                _mainWindowViewModel.ChangeViewModel(new FriendsViewModel(_mainWindowViewModel));
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while showing friends.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with showing friends.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while showing friends.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void JoinToRoomShow(object obj)
        {
            JoinRoomCode = string.Empty;
            IsJoinRoomDialogVisible = true;
        }

        public async void JoinToRoom(object obj)
        {
            try
            {
                roomViewModel = new RoomViewModel(_mainWindowViewModel);

                if (await roomViewModel.JoinToRoomAsync(JoinRoomCode))
                {
                    DisconnectPlayerFromChat();
                    _mainWindowViewModel.ChangeViewModel(roomViewModel);
                }
                else
                {
                    ErrorMessage = Properties.Resources.InvalidCode_Label;
                }
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while joining to room.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with join to room.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while joining to room.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void CancelJoinToRoom(object obj)
        {
            IsJoinRoomDialogVisible = false;
        }

        public async void CreateRoom(object obj)
        {
            try
            {
                roomViewModel = new RoomViewModel(_mainWindowViewModel);
                if (await roomViewModel.CreateARoomAsync())
                {
                    DisconnectPlayerFromChat();
                    _mainWindowViewModel.ChangeViewModel(roomViewModel);
                }
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while creating room.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with create room.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while creating room.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public async void LoadFriendsListAsync()
        {
            try
            {
                await _playerFriendsListServiceClient.GetConnectedFriendsAsync(UserId);
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while getting connected friends.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating getting connected friends.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error getting connected friends.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void ReceiveMessage(string username, string message)
        {
            Messages.Add($"{username} {message}");
            OnPropertyChanged(nameof(Messages));
        }

        public void ChatResponse(Service.OperationResult result)
        {
            if (!result.IsSuccess)
            {
                Log.Warn("An error was ocurred on lobby chat response");
            }
        }

        public async void PlayerFriendsList(PlayerFriendsResponse plalyerFriends)
        {
            try
            {
                if (plalyerFriends.Result.IsSuccess)
                {
                    var myFriends = plalyerFriends.FriendsIds;

                    foreach (var friendId in myFriends)
                    {
                        await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(friendId, UserId);
                    }
                }
                else
                {
                    Log.Warn($"Error to load friends list: {plalyerFriends.Result.Message}");
                }
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while getting friends list.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating getting friends list.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error getting friends list.", ex);
                IsServiceErrorVisible = true;
            }
        }


        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                var friend = new Player
                {
                    AccountId = response.PlayerInfo.PlayerInfo.Id,
                    Name = response.PlayerInfo.PlayerInfo.Name,
                    PicturePath = response.PlayerInfo.PlayerInfo.PicturePath
                };

                Friends.Add(friend);
            }
            else
            {
                Log.Warn("Error to load player information: " + response.Result.Message);
            }
        }

        public void SendRoomInvitationResponseCall(FriendService.OperationResult result)
        {
            if (!result.IsSuccess)
            {
                Log.Warn("An error was ocurred on lobby send room invitation response");
            }
            else
            {
                DisconnectPlayerFromChat();
                _mainWindowViewModel.ChangeViewModel(roomViewModel);
            }
        }

        public void ExecuteCloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
            _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
        }
    }
}
