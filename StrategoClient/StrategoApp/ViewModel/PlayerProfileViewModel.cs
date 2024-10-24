using StrategoApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace StrategoApp.ViewModel
{
    public class PlayerProfileViewModel : ViewModelBase
    {
        private string _username;
        private string _playerId;
        private string _imagePath;

        private readonly MainWindowViewModel _mainWindowViewModel;
        public bool IsOwnProfile { get; set; }
        public bool IsFriedged { get; set; }
        public bool CanEdit { get; set; }

        public ICommand EditProfilePictureCommand { get; }
        public ICommand EditUsernameCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand BackToLobby { get; }

        public PlayerProfileViewModel(MainWindowViewModel mainWindowViewModel)
        {
            if (PlayerSingleton.Instance.IsLoggedIn())
            {
                var player = PlayerSingleton.Instance.Player;
                Username = player.Name;
                PlayerId = player.Id.ToString();
                ImagePath = player.PicturePath;
            }
            else
            {
                Username = "Invitado";
                PlayerId = "-1";
                ImagePath = "pack://application:,,,/Assets/Images/ProfilePictures/Picture1.png";
            }

            _mainWindowViewModel = mainWindowViewModel;
            BackToLobby = new ViewModelCommand(ExecuteBackToLobby);
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

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged();
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
    }
}
