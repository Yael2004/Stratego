using log4net;
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

namespace StrategoApp.ViewModel
{
    public class PlayerProfileNotOwnViewModel : ViewModelBase, ProfileService.IOtherProfileDataServiceCallback, ProfileService.IPlayerFriendsListServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _username;
        private int _accountId;
        private int _playerId;
        private string _profilePicture;
        private string _playerTag;
        private int _gamesWon;
        private int _gamesLost;
        private int _gamesPlayed;
        private bool _isServiceErrorVisible;
        public ObservableCollection<int> FriendsIds { get; private set; }
        public bool IsFriend => FriendsIds.Contains(PlayerId);

        private MainWindowViewModel _mainWindowViewModel;

        private OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private PlayerFriendsListServiceClient _playerFriendsListServiceClient;

        public ICommand RemoveFriendCommand { get; }
        public ICommand BackToLobbyCommand { get; }

        public PlayerProfileNotOwnViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _accountId = PlayerSingleton.Instance.Player.Id;
            _mainWindowViewModel = mainWindowViewModel;
            FriendsIds = new ObservableCollection<int>();
            FriendsIds.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsFriend));

            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));
            _playerFriendsListServiceClient = new PlayerFriendsListServiceClient(new System.ServiceModel.InstanceContext(this));

            RemoveFriendCommand = new ViewModelCommand(RemoveFriend);
            BackToLobbyCommand = new ViewModelCommand(BackToLobby);

            LoadPlayerFriends();
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

        private async void LoadPlayerFriends()
        {
            try
            {
                await _playerFriendsListServiceClient.GetPlayerFriendsListAsync(_accountId);
            }
            catch (Exception ex)
            {
                Log.Error("Error al cargar la lista de amigos", ex);
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

        public async void LoadPlayerInfo(int playerId, int accountId)
        {
            MessageBox.Show($"Cargando perfil de jugador con ID: {playerId}", "Debug");
            await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(playerId, accountId);
        }

        public void PlayerFriendsList(PlayerFriendsResponse playerFriends)
        {
            if (playerFriends.Result.IsSuccess)
            {
                FriendsIds.Clear();

                foreach (var friendId in playerFriends.FriendsIds)
                {
                    FriendsIds.Add(friendId);
                }

                OnPropertyChanged(nameof(FriendsIds));
                OnPropertyChanged(nameof(IsFriend));
            }
            else
            {
                MessageBox.Show("No se pudo cargar la lista de amigos: " + playerFriends.Result.Message);
            }
        }
    }
}
