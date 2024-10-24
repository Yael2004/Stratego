using StrategoApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class PlayerProfileViewModel : ViewModelBase
    {
        private string _username;
        private string _playerId;

        private readonly MainWindowViewModel _mainWindowViewModel;
        public bool IsOwnProfile { get; set; }
        public bool IsFriedged { get; set; }
        public bool CanEdit { get; set; }

        public ICommand EditProfilePictureCommand { get; }
        public ICommand EditUsernameCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        public PlayerProfileViewModel(MainWindowViewModel mainWindowViewModel)
        {
            if (PlayerSingleton.Instance.IsLoggedIn())
            {
                var player = PlayerSingleton.Instance.Player;
                Username = player.Name;
                PlayerId = player.Id.ToString();
            }
            else
            {
                Username = "Invitado";
                PlayerId = "-1";
            }

            _mainWindowViewModel = mainWindowViewModel;
            //EditProfilePictureCommand = new ViewModelCommand(EditProfilePicture, CanEditProfilePicture);
            //EditUsernameCommand = new ViewModelCommand(EditUsername, CanEditUsername);
            //RemoveFriendCommand = new ViewModelCommand(RemoveFriend, CanRemoveFriend);
        }

        public PlayerProfileViewModel()
        {
            //EditProfilePictureCommand = new ViewModelCommand(EditProfilePicture, CanEditProfilePicture);
            //EditUsernameCommand = new ViewModelCommand(EditUsername, CanEditUsername);
            //RemoveFriendCommand = new ViewModelCommand(RemoveFriend, CanRemoveFriend);
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

        public string PlayerId
        {
            get { return _playerId; }
            set
            {
                _playerId = value;
                OnPropertyChanged();
            }
        }
    }
}
