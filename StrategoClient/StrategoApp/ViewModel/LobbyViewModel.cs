using log4net;
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
    public class LobbyViewModel : ViewModelBase, Service.IChatServiceCallback, ProfileService.IPlayerFriendsListServiceCallback, ProfileService.IOtherProfileDataServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _messageToSend;
        private string _errorMessage;
        private string _username;
        private string _profilePicture;
        private int _userId;

        private bool _isViewEnabled;
        private bool _isConnected = false;
        private bool _isJoinRoomDialogVisible = false;

        private string _joinRoomCode;

        private ChatServiceClient _chatClient;
        private OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private PlayerFriendsListServiceClient _playerFriendsListServiceClient;
        private MainWindowViewModel _mainWindowViewModel;
        private ObservableCollection<string> _messages;
        private ObservableCollection<Player> _friends;

        public ICommand SendMessagesCommand;
        public ICommand ShowProfileCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand JoinToRoomCommand { get; }
        public ICommand CancelJoinToRoomCommand { get; }
        public ICommand CreateRoomCommand { get; }
        public ICommand JoinToRoomShowCommand { get; }
        public ICommand ShowScoreboardCommand { get; }
        public ICommand ShowFriendsCommand { get; }

        public LobbyViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _chatClient = new ChatServiceClient(new InstanceContext(this));
            _playerFriendsListServiceClient = new PlayerFriendsListServiceClient(new InstanceContext(this));
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));

            AssignValuesToUser();
            Connection();

            _mainWindowViewModel = mainWindowViewModel;

            ShowFriendsCommand = new ViewModelCommand(ShowFriends);
            SendMessageCommand = new ViewModelCommand(ClientSendMessage, CanSendMessage);
            ShowProfileCommand = new ViewModelCommand(ClientShowProfile, CanShowProfile);
            JoinToRoomShowCommand = new ViewModelCommand(JoinToRoomShow);
            JoinToRoomCommand = new ViewModelCommand(JoinToRoom);
            CancelJoinToRoomCommand = new ViewModelCommand(CancelJoinToRoom);
            CreateRoomCommand = new ViewModelCommand(CreateRoom);
            ShowScoreboardCommand = new ViewModelCommand(ShowScoreboard);

            _messages = new ObservableCollection<string>();
            _friends = new ObservableCollection<Player>();

            LoadFriendsListAsync();
        }

        public void Connection()
        {
            if (!_isConnected)
            {
                try
                {
                    _userId = _chatClient.ConnectAsync(_userId, _username).Result;

                    PlayerSingleton.Instance.Player.Id = _userId;

                    MessageBox.Show($"{_username} conectado al chat.");
                }
                catch (FaultException ex)
                {
                    Log.Error("Error al conectar al servicio de chat", ex);
                    MessageBox.Show("No se pudo conectar al servicio de chat.");
                }
                catch (Exception ex)
                {
                    Log.Error("Error inesperado al conectar al chat", ex);
                    MessageBox.Show("Ha ocurrido un error inesperado.");
                }
            }
        }

        public void Disconnection()
        {
            if (_chatClient != null)
            {
                _chatClient.Disconnect(_userId);
                _chatClient.Close();
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

        public bool CanSendMessage(object obj)
        {
            return !string.IsNullOrWhiteSpace(MessageToSend);
        }

        public void ClientSendMessage(object obj)
        {
            if (_chatClient == null || _chatClient.State == CommunicationState.Closed || _chatClient.State == CommunicationState.Faulted)
            {
                Connection();
            }

            _chatClient.SendMessage(_userId, _username, MessageToSend);
            MessageToSend = string.Empty;
        }

        private bool CanShowProfile(object obj)
        {
            return true;
        }

        public void ClientShowProfile(object obj)
        {
            try
            {
                Disconnection();
                _mainWindowViewModel.ChangeViewModel(new PlayerProfileViewModel(_mainWindowViewModel));
            }
            catch (Exception ex)
            {
                Log.Error("Error al mostrar el perfil del jugador", ex);
            }
        }

        public void ShowScoreboard(object obj)
        {
            try
            {
                Disconnection();
                _mainWindowViewModel.ChangeViewModel(new ScoreboardViewModel(_mainWindowViewModel));
            }
            catch (Exception ex)
            {
                Log.Error("Error al mostrar el marcador", ex);
            }
        }

        public void ShowFriends(object obj)
        {
            try
            {
                Disconnection();
                _mainWindowViewModel.ChangeViewModel(new FriendsViewModel(_mainWindowViewModel));
            }
            catch (Exception ex)
            {
                Log.Error("Error al mostrar la lista de amigos", ex);
            }
        }

        public void JoinToRoomShow(object obj)
        {
            try
            {
                JoinRoomCode = string.Empty;
                IsJoinRoomDialogVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al unirse a la sala", ex);
            }
        }

        public async void JoinToRoom(object obj)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(JoinRoomCode))
                {
                    var roomViewModel = new RoomViewModel(_mainWindowViewModel);
                    
                    if (await roomViewModel.JoinToRoomAsync(JoinRoomCode))
                    {
                        Disconnection();
                        _mainWindowViewModel.ChangeViewModel(roomViewModel);
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, ingresa un código de sala válido.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al unirse a la sala", ex);
                MessageBox.Show("Error al unirse a la sala: " + ex.Message);
            }
        }

        public void CancelJoinToRoom(object obj)
        {
            try
            {
                IsJoinRoomDialogVisible = false;
            }
            catch (Exception ex)
            {
                Log.Error("Error al cancelar unirse a la sala", ex);
            }
        }

        public async void CreateRoom(object obj)
        {
            try
            {
                var roomViewModel = new RoomViewModel(_mainWindowViewModel);
                if (await roomViewModel.CreateARoomAsync())
                {
                    Disconnection();
                    _mainWindowViewModel.ChangeViewModel(roomViewModel);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear la sala", ex);
                MessageBox.Show("Error al crear la sala: " + ex.Message);
            }
        }

        public async void LoadFriendsListAsync()
        {
            try
            {
                //await _playerFriendsListServiceClient.GetPlayerFriendsListAsync(UserId);
                await _playerFriendsListServiceClient.GetConnectedFriendsAsync(UserId);
            }
            catch (Exception ex)
            {
                Log.Error("Error al cargar la lista de amigos", ex);
                MessageBox.Show("Error al cargar la lista de amigos.");
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
                ErrorMessage = "Error al enviar el mensaje";
            }
        }

        public async void PlayerFriendsList(PlayerFriendsResponse response)
        {
            if (response.Result.IsSuccess)
            {
                var myFriends = response.FriendsIds;
                
                foreach (var friendId in myFriends)
                {
                    await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(friendId, UserId);
                }
            }
            else
            {
                Log.Error($"Error al cargar la lista de amigos: {response.Result.Message}");
                MessageBox.Show($"Error al cargar la lista de amigos: {response.Result.Message}");
            }
        }


        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                MessageBox.Show(response.PlayerInfo.PlayerInfo.Name);
                //MessageBox.Show(response.PlayerInfo.PlayerInfo.PicturePath);

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
                Log.Error("Error al cargar información de amigo: " + response.Result.Message);
                MessageBox.Show("Error al cargar información de amigo: " + response.Result.Message);
            }
        }
    }
}
