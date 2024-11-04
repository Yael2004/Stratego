using log4net;
using StrategoApp.Helpers;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public PlayerProfileNotOwnViewModel()
        {
            RemoveFriendCommand = new ViewModelCommand(RemoveFriend);
        }

        public ICommand RemoveFriendCommand { get; }
        public ICommand BackToLobbyCommand { get; }


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
    }
}
