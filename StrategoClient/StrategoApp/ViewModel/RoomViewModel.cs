using StrategoApp.Model;
using StrategoApp.RoomService;
using StrategoApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class RoomViewModel : ViewModelBase, RoomService.IRoomServiceCallback
    {
        private RoomServiceClient _roomServiceClient;
        private string _username;
        private int _userId;
        private string _messageToSend;

        private ObservableCollection<string> _messages;

        private readonly MainWindowViewModel _mainWindowViewModel;
        public ICommand ExecuteBackToLobbyCommand { get; }
        public ICommand SendMessageCommand { get; }

        public string RoomCode { get; set; }

        public RoomViewModel(MainWindowViewModel mainWindowViewModel)
        {
            ExecuteBackToLobbyCommand = new ViewModelCommand(ExecuteBackToLobby);
            SendMessageCommand = new ViewModelCommand(SendMessageAsync);
            _messages = new ObservableCollection<string>();
            _mainWindowViewModel = mainWindowViewModel;
            InitializeService();
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

        public string MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                _messageToSend = value;
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
            }
        }

        public async Task CreateARoomAsync()
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                await _roomServiceClient.CreateRoomAsync(playerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating room: " + ex.Message);
            }
        }

        public async Task JoinToRoomAsync(string roomCode)
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                await _roomServiceClient.JoinRoomAsync(roomCode, playerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error joining room: " + ex.Message);
            }
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
    }
}
