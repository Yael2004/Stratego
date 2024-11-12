using log4net;
using StrategoApp.Helpers;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class PlayerProfileNotOwnViewModel : ViewModelBase, ProfileService.IOtherProfileDataServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _username;
        private int _playerId;
        private string _profilePicture;
        private string _playerTag;
        private int _gamesWon;
        private int _gamesLost;
        private int _gamesPlayed;

        private bool _isServiceErrorVisible;

        private MainWindowViewModel _mainWindowViewModel;

        private OtherProfileDataServiceClient _otherProfileDataServiceClient;

        public ICommand RemoveFriendCommand { get; }
        public ICommand BackToLobbyCommand { get; }

        public PlayerProfileNotOwnViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            MessageBox.Show("PlayerProfileNotOwnViewModel inicializado", "Debug");
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));
            RemoveFriendCommand = new ViewModelCommand(RemoveFriend);
            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
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

        private void BackToLobby(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        public void RemoveFriend(object obj)
        {
            var profileDataService = new ProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            PlayerId = response.PlayerInfo.PlayerInfo.Id;
            Username = response.PlayerInfo.PlayerInfo.Name;
            ProfilePicture = response.PlayerInfo.PlayerInfo.PicturePath;
            PlayerTag = response.PlayerInfo.PlayerInfo.LabelPath;
            GamesWon = response.PlayerInfo.PlayerStatistics.WonGames;
            GamesLost = response.PlayerInfo.PlayerStatistics.LostGames;
            GamesPlayed = response.PlayerInfo.PlayerStatistics.TotalGames;
        }

        public void LoadPlayerInfo(int playerId, int accountId)
        {
            MessageBox.Show($"Cargando perfil de jugador con ID: {playerId}", "Debug");
            _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(playerId, accountId);
        }
    }
}
