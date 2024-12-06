using log4net;
using StrategoApp.FriendService;
using StrategoApp.Helpers;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class FriendsViewModel : ViewModelBase, ProfileService.IPlayerFriendsListServiceCallback, ProfileService.IOtherProfileDataServiceCallback, FriendService.IPlayerFriendRequestServiceCallback, FriendService.IFriendOperationsServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _searchResult;
        private int _playerId;
        private int _friendIdRequested;
        private bool _isRequestsPopupOpen;
        private bool _isSearchPlayerPopupOpen;
        private bool _isServiceErrorVisible;
        private bool _isRequestSent;
        private Player _selectedFriend;


        private readonly PlayerFriendsListServiceClient _playerFriendsListServiceClient;
        private readonly OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private readonly PlayerFriendRequestServiceClient _playerFriendRequestServiceClient;
        private readonly FriendOperationsServiceClient _friendOperationsServiceClient;

        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly HashSet<int> pendingFriendRequests = new HashSet<int>();


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
        public ICommand ExecuteCloseServiceErrorCommand { get; }
        public ICommand ExecuteCloseRequestSent { get; }

        public string SearchResult
        {
            get { return _searchResult; }
            set
            {
                _searchResult = value;
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

        public bool IsServiceErrorVisible
        {
            get { return _isServiceErrorVisible; }
            set
            {
                _isServiceErrorVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsRequestSent
        {
            get { return _isRequestSent; }
            set
            {
                _isRequestSent = value;
                OnPropertyChanged();
            }
        }


        public FriendsViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            AssignValuesToUser();

            Friends = new ObservableCollection<Player>();
            FriendRequests = new ObservableCollection<Player>();

            _playerFriendsListServiceClient = new PlayerFriendsListServiceClient(new InstanceContext(this));
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _playerFriendRequestServiceClient = new PlayerFriendRequestServiceClient(new InstanceContext(this));
            _friendOperationsServiceClient = new FriendOperationsServiceClient(new InstanceContext(this));

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            ViewProfileCommand = new ViewModelCommand(ViewProfile);
            LoadRequestsCommand = new ViewModelCommand(LoadRequests);
            CloseRequestsPopupCommand = new ViewModelCommand(CloseRequestsPopup);
            OpenSearchPlayerPopupCommand = new ViewModelCommand(OpenSearchPlayerPopup);
            AcceptSendRequestCommand = new ViewModelCommand(SearchPlayerById);
            CancelSearchCommand = new ViewModelCommand(CloseSearchPlayerPopup);
            AcceptFriendRequestCommand = new ViewModelCommand(AcceptFriendRequest);
            DeclineFriendRequestCommand = new ViewModelCommand(RejectFriendRequest);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(CloseServiceError);
            ExecuteCloseRequestSent = new ViewModelCommand(CloseRequestSend);

            LoadFriendsListAsync();

            IsRequestsPopupOpen = false;
            IsServiceErrorVisible = false;
            IsRequestSent = false;
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
                catch (CommunicationException cex)
                {
                    Log.Error($"Communication error while loading player info: {cex.Message}.");
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error($"Timed out while loading player info: {tex.Message}");
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Unexpected error while loading player info: {ex.Message}.");
                    IsServiceErrorVisible = true;
                }
            }
            else
            {
                Log.Warn("The parameter is not a player object");
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
            catch (CommunicationException cex)
            {
                Log.Error($"Communication error while getting player friends list: {cex.Message}.");
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException tex)
            {
                Log.Error($"Timed out while getting player friends list: {tex.Message}");
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected error while getting player friends list: {ex.Message}.");
                IsServiceErrorVisible = true;
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
            catch (CommunicationException cex)
            {
                Log.Error($"Communication error while getting friend requsts: {cex.Message}.");
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException tex)
            {
                Log.Error($"Timed out while getting friend requsts: {tex.Message}");
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected error while getting friend requsts: {ex.Message}.");
                IsServiceErrorVisible = true;
            }
        }

        public async void PlayerFriendsList(PlayerFriendsResponse response)
        {
            if (response.Result.IsSuccess)
            {
                var myFriends = response.FriendsIds;

                try
                {
                    foreach (var friendId in myFriends)
                    {
                        await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(friendId, PlayerId);
                    }
                }
                catch (CommunicationException cex)
                {
                    Log.Error($"Communication error while getting other player info: {cex.Message}.");
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error($"Timed out while getting other player info: {tex.Message}");
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Unexpected error while getting other player info: {ex.Message}.");
                    IsServiceErrorVisible = true;
                }
            }
            else
            {
                Log.Warn($"Failed to load friend list: {response.Result.Message}");
            }
        }

        private async void SearchPlayerById(object obj)
        {
            try
            {
                await _friendOperationsServiceClient.SendFriendRequestAsync(FriendIdRequested, PlayerId);
            }
            catch (CommunicationException cex)
            {
                Log.Error($"Communication error while seinding friend request: {cex.Message}.");
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException tex)
            {
                Log.Error($"Timed out while seinding friend request: {tex.Message}");
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected error while seinding friend request: {ex.Message}.");
                IsServiceErrorVisible = true;
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
                }
                catch (CommunicationException cex)
                {
                    Log.Error($"Communication error while accepting friend request: {cex.Message}.");
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error($"Timed out while accepting friend request: {tex.Message}");
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Unexpected error while accepting friend request: {ex.Message}.");
                    IsServiceErrorVisible = true;
                }
            }
            else
            {
                Log.Warn("Parameter is not valid on AcceptFriendRequest");
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
                }
                catch (CommunicationException cex)
                {
                    Log.Error($"Communication error while declining friend request: {cex.Message}.");
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error($"Timed out while declining friend request: {tex.Message}");
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Unexpected error while declining friend request: {ex.Message}.");
                    IsServiceErrorVisible = true;
                }
            }
            else
            {
                Log.Warn("Parameter is not valid on AcceptFriendRequest");
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
                Log.Warn("Failed to load player information: " + response.Result.Message);
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

                try
                {
                    foreach (var friendRequestId in response.FriendRequestIds)
                    {
                        pendingFriendRequests.Add(friendRequestId);
                        _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(friendRequestId, PlayerId);
                    }
                }
                catch (CommunicationException cex)
                {
                    Log.Error($"Communication error while getting friend request ids: {cex.Message}.");
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error($"Timed out while getting friend request ids: {tex.Message}");
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Unexpected error while getting friend request ids: {ex.Message}.");
                    IsServiceErrorVisible = true;
                }
            }
            else
            {
                Log.Warn("Failed to load friend requests: " + response.Result.Message);
            }
        }

        public void GetFriendOperationSend(FriendService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                SearchResult = string.Empty;
                IsSearchPlayerPopupOpen = false;
                IsRequestSent = true;
            }
            else
            {
                SearchResult = Properties.Resources.InvalidPlayerId;
            }
        }

        public void GetFriendOperationAccept(FriendService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                Log.Info("Request accepted");
            }
            else
            {
                Log.Warn($"Failed to accept request: {result.Message}");
            }
        }

        public void GetFriendOperationDecline(FriendService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                Log.Info("Request declined");
            }
            else
            {
                Log.Warn($"Failed to decline request: {result.Message}");
            }
        }

        public void GetFriendOperationRemove(FriendService.OperationResult result)
        {
            if (result.IsSuccess)
            {
                Log.Info("Friend removed");
            }
            else
            {
                Log.Warn($"Failed to remove friend: {result.Message}");
            }
        }

        private void CloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        private void CloseRequestSend(object obj)
        {
            IsRequestSent = false;
        }
    }
}
