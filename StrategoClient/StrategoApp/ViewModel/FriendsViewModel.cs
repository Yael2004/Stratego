using StrategoApp.Helpers;
using StrategoApp.Model;
using StrategoApp.ProfileService;
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
    public class FriendsViewModel : ViewModelBase, ProfileService.IPlayerFriendsListServiceCallback, ProfileService.IOtherProfileDataServiceCallback
    {
        private MainWindowViewModel _mainWindowViewModel;

        private PlayerFriendsListServiceClient _playerFriendsListServiceClient;
        private OtherProfileDataServiceClient _otherProfileDataServiceClient;

        private int _playerId;
        private Player _selectedFriend;
        private bool _isRequestsPopupOpen;
        private bool _isSearchPlayerPopupOpen;

        public ObservableCollection<Player> Friends { get; set; }

        public ICommand ViewProfileCommand { get; }
        public ICommand BackToLobbyCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand CloseRequestsPopupCommand { get; }
        public ICommand OpenSearchPlayerPopupCommand { get; }
        public ICommand CancelSearchCommand { get; }

        public FriendsViewModel(MainWindowViewModel mainWindowViewModel)
        {
            AssignValuesToUser();
            Friends = new ObservableCollection<Player>();

            _playerFriendsListServiceClient = new PlayerFriendsListServiceClient(new System.ServiceModel.InstanceContext(this));
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));
            _mainWindowViewModel = new MainWindowViewModel();

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            ViewProfileCommand = new ViewModelCommand(ViewProfile);
            LoadRequestsCommand = new ViewModelCommand(LoadRequests);
            CloseRequestsPopupCommand = new ViewModelCommand(CloseRequestsPopup);
            OpenSearchPlayerPopupCommand = new ViewModelCommand(OpenSearchPlayerPopup);
            CancelSearchCommand = new ViewModelCommand(CloseSearchPlayerPopup);

            LoadFriendsListAsync();

            IsRequestsPopupOpen = false;
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

        public Player SelectedFriend
        {
            get { return _selectedFriend; }
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
            }
        }

        public bool IsRequestsPopupOpen
        {
            get { return _isRequestsPopupOpen; }
            set
            {
                _isRequestsPopupOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsSearchPlayerPopupOpen
        {
            get { return _isSearchPlayerPopupOpen; }
            set
            {
                _isSearchPlayerPopupOpen = value;
                OnPropertyChanged();
            }
        }

        private void ViewProfile(object obj)
        {
            if (obj is Player friend)
            {
                try
                {
                    var playerProfileNotOwnViewModel = new PlayerProfileNotOwnViewModel(_mainWindowViewModel);

                    playerProfileNotOwnViewModel.LoadPlayerInfo(friend.AccountId, PlayerId);
                    _mainWindowViewModel.ChangeViewModel(playerProfileNotOwnViewModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar perfil de jugador: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("El parámetro no es un objeto Player.");
            }
        }


        private void BackToLobby(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        public async void LoadFriendsListAsync()
        {
            try
            {
                await _playerFriendsListServiceClient.GetPlayerFriendsListAsync(PlayerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de amigos.");
            }
        }

        private void CloseRequestsPopup(object obj)
        {
            IsRequestsPopupOpen = false;
        }

        private void OpenSearchPlayerPopup(object obj)
        {
            IsSearchPlayerPopupOpen = true;
        }

        private void CloseSearchPlayerPopup(object obj)
        {
            IsSearchPlayerPopupOpen = false;
        }

        private async void LoadRequests(object obj)
        {
            try
            {
                /*var requests = await _playerFriendsListServiceClient.GetFriendRequestsAsync(PlayerId);
                FriendRequests.Clear();
                foreach (var request in requests)
                {
                    FriendRequests.Add(request);
                }*/

                IsRequestsPopupOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar solicitudes de amistad.");
            }
        }

        public async void PlayerFriendsList(PlayerFriendsResponse response)
        {
            if (response.Result.IsSuccess)
            {
                var myFriends = response.FriendsIds;

                foreach (var friendId in myFriends)
                {
                    await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(friendId, PlayerId);
                }
            }
            else
            {
                MessageBox.Show($"Error al cargar la lista de amigos: {response.Result.Message}");
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
                MessageBox.Show("Error al cargar información de amigo: " + response.Result.Message);
            }
        }

        public void AssignValuesToUser()
        {
            if (PlayerSingleton.Instance.IsLoggedIn())
            {
                var player = PlayerSingleton.Instance.Player;
                PlayerId = player.Id;
            }
        }
    }
}
