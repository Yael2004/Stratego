using log4net;
using StrategoApp.Helpers;
using StrategoApp.Model;
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
    public class LobbyViewModel : ViewModelBase, Service.IChatServiceCallback
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

        private static LobbyViewModel _instance;

        private ChatServiceClient _chatClient;
        private MainWindowViewModel _mainWindowViewModel;
        private ObservableCollection<string> _messages;

        public ICommand SendMessagesCommand;
        public ICommand ShowProfileCommand { get;  }
        public ICommand SendMessageCommand { get; }
        public ICommand JoinToRoomCommand { get; }
        public ICommand CancelJoinToRoomCommand { get; }
        public ICommand CreateRoomCommand { get; }
        public ICommand JoinToRoomShowCommand { get; }

        public LobbyViewModel(MainWindowViewModel mainWindowViewModel)
        {
            AssignValuesToUser();

            if (!_isConnected)
            {
                Connection();
            }

            _mainWindowViewModel = mainWindowViewModel;
            SendMessageCommand = new ViewModelCommand(ClientSendMessage, CanSendMessage);
            ShowProfileCommand = new ViewModelCommand(ClientShowProfile, CanShowProfile);
            JoinToRoomShowCommand = new ViewModelCommand(JoinToRoomShow);
            JoinToRoomCommand = new ViewModelCommand(JoinToRoom);
            CancelJoinToRoomCommand = new ViewModelCommand(CancelJoinToRoom);
            CreateRoomCommand = new ViewModelCommand(CreateRoom);
            _messages = new ObservableCollection<string>();
        }

        public LobbyViewModel()
        {
            {
                _messages = new ObservableCollection<string>();
                SendMessagesCommand = new ViewModelCommand(ClientSendMessage, CanSendMessage);
                ShowProfileCommand = new ViewModelCommand(ClientShowProfile, CanShowProfile);
            }
        }

        public void Connection()
        {
            if (!_isConnected)
            {
                try
                {
                    InstanceContext context = new InstanceContext(this);
                    _chatClient = new ChatServiceClient(context);

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

        ~LobbyViewModel()
        {
            Disconnection();
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
                    await roomViewModel.JoinToRoomAsync(JoinRoomCode);
                    _mainWindowViewModel.ChangeViewModel(roomViewModel);
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
                await roomViewModel.CreateARoomAsync();
                _mainWindowViewModel.ChangeViewModel(roomViewModel);
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear la sala", ex);
                MessageBox.Show("Error al crear la sala: " + ex.Message);
            }
        }

        public void ReceiveMessage(string username, string message)
        {
            Messages.Add($"{username} {message}");
            OnPropertyChanged(nameof(Messages));
        }

        public void ChatResponse(OperationResult result)
        {
            if (!result.IsSuccess)
            {
                ErrorMessage = "Error al enviar el mensaje";
            }
        }
    }
}
