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
    public class FriendsViewModel : ViewModelBase, ProfileService.IPlayerFriendsListServiceCallback
    {
        private MainWindowViewModel _mainWindowViewModel;

        private PlayerFriendsListServiceClient _playerFriendsListServiceClient;
        private OtherProfileDataServiceClient _otherProfileDataServiceClient;

        private int _userId;
        public ObservableCollection<Player> Friends { get; set; }

        public ICommand InviteCommand { get; }
        public ICommand ViewProfileCommand { get; }
        public ICommand BackToLobbyCommand { get; }

        public FriendsViewModel(MainWindowViewModel mainWindowViewModel)
        {
            Friends = new ObservableCollection<Player>();

            _playerFriendsListServiceClient = new PlayerFriendsListServiceClient(new System.ServiceModel.InstanceContext(this));
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new System.ServiceModel.InstanceContext(this));
            _mainWindowViewModel = new MainWindowViewModel();

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            InviteCommand = new ViewModelCommand(InviteFriend);
            ViewProfileCommand = new ViewModelCommand(ViewProfile);

            UserId = PlayerSingleton.Instance.Player.AccountId;
            _ = LoadFriendsListAsync();
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

        private void InviteFriend(object obj)
        {

        }

        private void ViewProfile(object obj)
        {

        }

        private void BackToLobby(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }

        public async Task LoadFriendsListAsync()
        {
            try
            {
                await _playerFriendsListServiceClient.GetPlayerFriendsListAsync(UserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de amigos.");
            }
        }

        public void PlayerFriendsList(PlayerFriendsResponse response)
        {
            if (response.Result.IsSuccess)
            {
                foreach (var friendId in response.FriendsIds)
                {
                    _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(friendId, UserId);
                }
            }
            else
            {
                MessageBox.Show($"Error al cargar la lista de amigos: {response.Result.Message}");
            }
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess && response.PlayerInfo != null)
            {
                var friend = new Player
                {
                    AccountId = response.PlayerInfo.PlayerInfo.Id,
                    Name = response.PlayerInfo.PlayerInfo.Name,
                    PicturePath = response.PlayerInfo.PlayerInfo.PicturePath
                };

                if (!Friends.Any(f => f.AccountId == friend.AccountId))
                {
                    Friends.Add(friend);
                }
            }
            else
            {
                MessageBox.Show("Error al cargar información de amigo: " + response.Result.Message);
            }
        }
    }
}
