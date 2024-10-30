using StrategoApp.Model;
using System;
using StrategoApp.ProfileService;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace StrategoApp.ViewModel
{
    public class PlayerProfileViewModel : ViewModelBase
    {
        private string _username;
        private int _playerId;
        private string _profilePicture;
        private string _selectedProfilePicture;

        private bool _isProileSelectorVisible;
        public ObservableCollection<string> ProfilePictures { get; set; }

        private readonly MainWindowViewModel _mainWindowViewModel;
        public bool IsOwnProfile { get; set; }
        public bool IsFriedged { get; set; }
        public bool CanEdit { get; set; }

        public ICommand EditProfilePictureCommand { get; }
        public ICommand EditUsernameCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand BackToLobbyCommand { get; }
        public ICommand SelectProfilePictureCommand { get; }
        public ICommand ToggleProfileSelectorVisibilityCommand { get; }
        public ICommand CancelProfileSelectionCommand { get; }
        public ICommand LogoutCommand { get; }

        public PlayerProfileViewModel(MainWindowViewModel mainWindowViewModel)
        {
            if (PlayerSingleton.Instance.IsLoggedIn())
            {
                var player = PlayerSingleton.Instance.Player;
                Username = player.Name;
                PlayerId = player.Id;
                ProfilePicture = player.PicturePath;
            }
            else
            {
                Username = "Invited";
                PlayerId = 0;
                ProfilePicture = "pack://application:,,,/Assets/Images/ProfilePictures/Picture1.png";
            }

            ProfilePictures = new ObservableCollection<string>
            {
                "pack://application:,,,/Assets/Images/ProfilePictures/Picture1.png",
                "pack://application:,,,/Assets/Images/ProfilePictures/Picture2.png",
                "pack://application:,,,/Assets/Images/ProfilePictures/Picture3.png",
                "pack://application:,,,/Assets/Images/ProfilePictures/Picture4.png",
                "pack://application:,,,/Assets/Images/ProfilePictures/Picture5.png",
                "pack://application:,,,/Assets/Images/ProfilePictures/Picture6.png"
            };

            _mainWindowViewModel = mainWindowViewModel;
            BackToLobbyCommand = new ViewModelCommand(ExecuteBackToLobby);
            //EditProfilePictureCommand = new ViewModelCommand(EditProfilePicture, CanEditProfilePicture);
            //EditUsernameCommand = new ViewModelCommand(EditUsername, CanEditUsername);
            //RemoveFriendCommand = new ViewModelCommand(RemoveFriend, CanRemoveFriend);
            SelectProfilePictureCommand = new ViewModelCommand(SelectProfilePicture);
            ToggleProfileSelectorVisibilityCommand = new ViewModelCommand(ToggleProfileSelectorVisibility);
            CancelProfileSelectionCommand = new ViewModelCommand(CancelProfileSelection);
            LogoutCommand = new ViewModelCommand(Logout);
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

        public string SelectedProfilePicture
        {
            get { return _selectedProfilePicture; }
            set
            {
                _selectedProfilePicture = value;
                OnPropertyChanged();
            }
        }

        public bool IsProfileSelectorVisible
        {
            get { return _isProileSelectorVisible; }
            set
            {
                _isProileSelectorVisible = value;
                OnPropertyChanged();
            }
        }

        private void ExecuteBackToLobby(Object obj)
        {
            try
            { 
                _mainWindowViewModel.ChangeViewModel(LobbyViewModel.Instance(_mainWindowViewModel)); 
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message); 
            }
        }

        private void SelectProfilePicture(Object obj)
        {
            SelectedProfilePicture = obj.ToString();
        }

        public void ToggleProfileSelectorVisibility(Object obj)
        {
            IsProfileSelectorVisible = !IsProfileSelectorVisible;
        }

        public void CancelProfileSelection(Object obj)
        {
            IsProfileSelectorVisible = false;
        }

        public void Logout(Object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
        }
    }
}
