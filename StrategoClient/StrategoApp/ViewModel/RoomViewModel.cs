using StrategoApp.Model;
using StrategoApp.RoomService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private ObservableCollection<string> _messages;

        private readonly MainWindowViewModel _mainWindowViewModel;
        public ICommand ExecuteBackToLobbyCommand { get; }

        public string RoomCode { get; set; }
        public string Message { get; set; }

        public RoomViewModel(MainWindowViewModel mainWindowViewModel)
        {
            ExecuteBackToLobbyCommand = new ViewModelCommand(ExecuteBackToLobby);
            _mainWindowViewModel = mainWindowViewModel;
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

        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged();
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

        public async Task CreateRoomAsync()
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                await _roomServiceClient.CreateRoomAsync("" + playerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating room: " + ex.Message);
            }
        }

        public async Task JoinRoomAsync(string roomCode)
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                await _roomServiceClient.JoinRoomAsync("" + playerId, roomCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error joining room: " + ex.Message);
            }
        }

        private void LeaveRoomAsync()
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                _roomServiceClient.LeaveRoomAsync("" + playerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error leaving room: " + ex.Message);
            }
        }

        private async Task SendMessageAsync(string message)
        {
            try
            {
                var playerId = PlayerSingleton.Instance.Player.Id;
                await _roomServiceClient.SendMessageToRoomAsync("" + playerId, RoomCode, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending message: " + ex.Message);
            }
        }

        private void ExecuteBackToLobby(Object obj)
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

        public void RoomResponseAsync(OperationResult response)
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

        public void ReceiveMessageAsync(string playerId, string message)
        {
            MessageBox.Show($"Message from {playerId}: {message}");
        }
    }
}
