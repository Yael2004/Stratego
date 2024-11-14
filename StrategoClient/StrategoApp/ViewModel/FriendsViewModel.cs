using StrategoApp.FriendService;
using StrategoApp.Helpers;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class FriendsViewModel : ViewModelBase, ProfileService.IPlayerFriendsListServiceCallback, ProfileService.IOtherProfileDataServiceCallback, FriendService.IPlayerFriendRequestServiceCallback, FriendService.IFriendOperationsServiceCallback
    {
        private HashSet<int> pendingFriendRequests = new HashSet<int>();
        private readonly MainWindowViewModel _mainWindowViewModel;

        private PlayerFriendsListServiceClient _playerFriendsListServiceClient;
        private OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private PlayerFriendRequestServiceClient _playerFriendRequestServiceClient;
        private FriendOperationsServiceClient _friendOperationsServiceClient;

        private int _playerId;
        private Player _selectedFriend;
        private int _friendIdRequested;
        private bool _isRequestsPopupOpen;
        private bool _isSearchPlayerPopupOpen;

        public ObservableCollection<Player> Friends { get; set; }
        public ObservableCollection<Player> FriendRequests { get; set; }

        public ICommand ViewProfileCommand { get; }
        public ICommand BackToLobbyCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand CloseRequestsPopupCommand { get; }
        public ICommand OpenSearchPlayerPopupCommand { get; }
        public ICommand AcceptSendRequestCommand { get; }
        public ICommand CancelSearchCommand { get; }
        public ICommand AcceptFriendRequestCommand { get; }
        public ICommand DeclineFriendRequestCommand { get; }

        public FriendsViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            AssignValuesToUser();

            Friends = new ObservableCollection<Player>();
            FriendRequests = new ObservableCollection<Player>();

            _playerFriendsListServiceClient = new PlayerFriendsListServiceClient(new System.ServiceModel.InstanceContext(this));
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));
            _playerFriendRequestServiceClient = new PlayerFriendRequestServiceClient(new System.ServiceModel.InstanceContext(this));
            _friendOperationsServiceClient = new FriendOperationsServiceClient(new System.ServiceModel.InstanceContext(this));

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            ViewProfileCommand = new ViewModelCommand(ViewProfile);
            LoadRequestsCommand = new ViewModelCommand(LoadRequests);
            CloseRequestsPopupCommand = new ViewModelCommand(CloseRequestsPopup);
            OpenSearchPlayerPopupCommand = new ViewModelCommand(OpenSearchPlayerPopup);
            AcceptSendRequestCommand = new ViewModelCommand(SearchPlayerById);
            CancelSearchCommand = new ViewModelCommand(CloseSearchPlayerPopup);
            AcceptFriendRequestCommand = new ViewModelCommand(AcceptFriendRequest);
            DeclineFriendRequestCommand = new ViewModelCommand(RejectFriendRequest);

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

        public int FriendIdRequested
        {
            get { return _friendIdRequested; }
            set
            {
                _friendIdRequested = value;
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
                FriendRequests.Clear();
                await _playerFriendRequestServiceClient.GetPlayerFriendRequestAsync(PlayerId);

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

        private async void SearchPlayerById(object obj)
        {
            try
            {
                await _friendOperationsServiceClient.SendFriendRequestAsync(FriendIdRequested, PlayerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar jugador por ID.");
            }
        }

        private async void AcceptFriendRequest(object obj)
        {
            if (obj is Player friend && friend.AccountId > 0)
            {
                try
                {
                    await _friendOperationsServiceClient.AcceptFriendRequestAsync(friend.AccountId, PlayerId);
                    FriendRequests.Remove(friend);
                    MessageBox.Show("Solicitud de amistad aceptada.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al aceptar solicitud de amistad.");
                }
            }
            else
            {
                MessageBox.Show("El parámetro no es un objeto Player válido o el ID es 0.");
            }
        }

        private async void RejectFriendRequest(object obj)
        {
            if (obj is Player friend && friend.AccountId > 0)
            {
                try
                {
                    await _friendOperationsServiceClient.DeclineFriendRequestAsync(friend.AccountId, PlayerId);
                    FriendRequests.Remove(friend);
                    MessageBox.Show("Solicitud de amistad rechazada.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al rechazar solicitud de amistad.");
                }
            }
            else
            {
                MessageBox.Show("El parámetro no es un objeto Player válido o el ID es 0.");
            }
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                var player = new Player
                {
                    Id = response.PlayerInfo.PlayerInfo.Id,
                    AccountId = response.PlayerInfo.PlayerInfo.Id,
                    Name = response.PlayerInfo.PlayerInfo.Name,
                    PicturePath = response.PlayerInfo.PlayerInfo.PicturePath
                };

                if (pendingFriendRequests.Contains(player.Id))
                {
                    FriendRequests.Add(player);
                    pendingFriendRequests.Remove(player.Id);
                }
                else
                {
                    Friends.Add(player);
                }
            }
            else
            {
                MessageBox.Show("Error al cargar información del jugador: " + response.Result.Message);
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

        public void ReceiveFriendRequestIds(PlayerFriendRequestResponse response)
        {
            if (response.Result.IsSuccess)
            {
                FriendRequests.Clear();

                foreach (var friendRequestId in response.FriendRequestIds)
                {
                    pendingFriendRequests.Add(friendRequestId);
                    _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(friendRequestId, PlayerId);
                }
            }
            else
            {
                MessageBox.Show("Error al cargar solicitudes de amistad: " + response.Result.Message);
            }
        }

        public void GetFriendOperation(FriendService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                MessageBox.Show("Solicitud de amistad enviada.");
            }
            else
            {
                MessageBox.Show("Error al enviar solicitud de amistad: " + result.Message);
            }
        }
    }
}
