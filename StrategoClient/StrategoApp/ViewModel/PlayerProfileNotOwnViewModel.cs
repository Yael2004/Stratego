using log4net;
using StrategoApp.FriendService;
using StrategoApp.Helpers;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace StrategoApp.ViewModel
{
    public class PlayerProfileNotOwnViewModel : ViewModelBase, ProfileService.IOtherProfileDataServiceCallback, FriendService.IFriendRemoveServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _username;
        private string _profilePicture;
        private string _playerTag;
        private string _exceptionMessage;
        private int _accountId;
        private int _playerId;
        private int _gamesWon;
        private int _gamesLost;
        private int _gamesPlayed;
        private bool _isServiceErrorVisible;
        private bool _isFriend;

        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private readonly FriendRemoveServiceClient _friendRemoveServiceClient;
        private readonly PingCheck _pingCheck;

        public ICommand RemoveFriendCommand { get; }
        public ICommand BackToLobbyCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public int PlayerId
        {
            get { return _playerId; }
            set
            {
                _playerId = value;
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

        public string PlayerTag
        {
            get { return _playerTag; }
            set
            {
                _playerTag = value;
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

        public int AccountId
        {
            get { return _accountId; }
            set
            {
                _accountId = value;
                OnPropertyChanged();
            }
        }

        public int GamesWon
        {
            get { return _gamesWon; }
            set
            {
                _gamesWon = value;
                OnPropertyChanged();
            }
        }

        public int GamesLost
        {
            get { return _gamesLost; }
            set
            {
                _gamesLost = value;
                OnPropertyChanged();
            }
        }

        public int GamesPlayed
        {
            get { return _gamesPlayed; }
            set
            {
                _gamesPlayed = value;
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

        public bool IsFriend
        {
            get { return _isFriend; }
            set
            {
                _isFriend = value;
                OnPropertyChanged();
            }
        }

        public PlayerProfileNotOwnViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _friendRemoveServiceClient = new FriendRemoveServiceClient(new InstanceContext(this));
            
            _mainWindowViewModel = mainWindowViewModel;

            _pingCheck = new PingCheck(_mainWindowViewModel);
            Task.Run(() => _pingCheck.StartPingMonitoringAsync());
            
            RemoveFriendCommand = new ViewModelCommand(RemoveFriend);
            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(CloseServiceError);

            AccountId = PlayerSingleton.Instance.Player.Id;
            IsFriend = false;
        }

        private void BackToLobby(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            _pingCheck.StopPingMonitoring();
        }

        public void RemoveFriend(object obj)
        {
            try
            {
                _friendRemoveServiceClient.RemoveFriendAsync(PlayerId, PlayerSingleton.Instance.Player.Id);
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while removing friend.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
            _pingCheck.StopPingMonitoring();
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while removing friend.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while removing friend.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (!response.Result.IsSuccess)
            {
                return;
            }
            else if (response.Result.IsDataBaseError)
            {
                Log.Error($"Database error ocurred on GerFriendOperationRemove {response.Result.Message}");
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
                return;
            }

            PlayerId = response.PlayerInfo.PlayerInfo.Id;
            Username = response.PlayerInfo.PlayerInfo.Name;
            ProfilePicture = response.PlayerInfo.PlayerInfo.PicturePath;
            GamesWon = response.PlayerInfo.PlayerStatistics.WonGames;
            GamesLost = response.PlayerInfo.PlayerStatistics.LostGames;
            GamesPlayed = response.PlayerInfo.PlayerStatistics.TotalGames;

            if (response.PlayerInfo.IsFriend)
            {
                IsFriend = true;
            }
            

            var playerTag = response.PlayerInfo.PlayerInfo.LabelPath;

            switch (playerTag)
            {
                case "label1":
                    PlayerTag = Properties.Resources.NovicePlayer_Label;
                    break;

                case "label2":
                    PlayerTag = Properties.Resources.ProPlayer_Label;
                    break;

                case "label3":
                    PlayerTag = Properties.Resources.Apprentice_Label;
                    break;

                case "label4":
                    PlayerTag = Properties.Resources.Competitive_Label;
                    break;

                default:
                    PlayerTag = Properties.Resources.NovicePlayer_Label;
                    break;
            }
        }

        public async void LoadPlayerInfo(int playerId, int accountId)
        {
            try
            {
                await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(playerId, accountId);
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while getting other player info.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while getting other player info.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while getting other player info.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
        }

        private void CloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
        }

        public void GetFriendOperationRemove(FriendService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                IsFriend = false;
            }
            else if (result.IsDataBaseError)
            {
                Log.Error($"Database error ocurred on GerFriendOperationRemove {result.Message}");
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
            else
            {
                Log.Warn("Error on getFriendOperationRemove, result was false");
            }
        }

    }
}
