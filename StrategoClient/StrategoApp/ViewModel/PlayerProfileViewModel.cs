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
using System.ServiceModel;
using System.Configuration;
using StrategoApp.Helpers;
using StrategoApp.LogInService;
using System.Windows;
using System.Web.UI.WebControls;
using log4net;

namespace StrategoApp.ViewModel
{
    public class PlayerProfileViewModel : ViewModelBase, ProfileService.IProfileDataServiceCallback, ProfileService.IProfileModifierServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _username;
        private string _usernameEdited;
        private int _playerId;
        private string _profilePicture;
        private string _playerTag;
        private int _gamesWon;
        private int _gamesLost;
        private int _gamesPlayed;
        private string _selectedProfilePicture;
        
        private string _usernameError;

        private bool _isServiceErrorVisible;
        private bool _isProileSelectorVisible;
        private bool _isEditUsernameVisible;
        public ObservableCollection<string> ProfilePictures { get; set; }
        public ObservableCollection<string> ProfileTags { get; set; }

        private readonly MainWindowViewModel _mainWindowViewModel;
        public bool IsOwnProfile { get; set; }
        public bool IsFriedged { get; set; }
        public bool CanEdit { get; set; }

        public ICommand EditProfilePictureCommand { get; }
        public ICommand EditUsernameCommand { get; }
        public ICommand BackToLobbyCommand { get; }
        public ICommand SelectProfilePictureCommand { get; }
        public ICommand ToggleProfileSelectorVisibilityCommand { get; }
        public ICommand ToggleEditUsernameVisibilityCommand { get; }
        public ICommand CancelProfileSelectionCommand { get; }
        public ICommand CancelEditUsernameCommand { get; }
        public ICommand AcceptEditUsernameCommand { get; }
        public ICommand SaveProfilePictureSelectionCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }

        public PlayerProfileViewModel(MainWindowViewModel mainWindowViewModel)
        {
            PlayerInfoResponse playerInfoResponse = new PlayerInfoResponse();
            PlayerStatisticsResponse playerStatisticsResponse = new PlayerStatisticsResponse();

            LoadPlayerInfoFromSingleton();
            LoadPlayerStatisticsAsync();

            ProfileTags = new ObservableCollection<string>
            {
                Properties.Resources.NovicePlayer_Label,
                Properties.Resources.ProPlayer_Label,
                Properties.Resources.Apprentice_Label,
                Properties.Resources.Competitive_Label
            };

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
            SelectProfilePictureCommand = new ViewModelCommand(SelectProfilePicture);
            ToggleProfileSelectorVisibilityCommand = new ViewModelCommand(ToggleProfileSelectorVisibility);
            ToggleEditUsernameVisibilityCommand = new ViewModelCommand(ToggleEditUsernameVisibility);
            CancelProfileSelectionCommand = new ViewModelCommand(CancelProfileSelection);
            CancelEditUsernameCommand = new ViewModelCommand(CancelEditUsername);
            AcceptEditUsernameCommand = new ViewModelCommand(AcceptEditUsername, CanAcceptEditUsername);
            SaveProfilePictureSelectionCommand = new ViewModelCommand(ConfirmProfileSelection);
            LogoutCommand = new ViewModelCommand(Logout);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(ExecuteCloseServerError);
            IsServiceErrorVisible = false;
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

        public string UsernameEdited
        {
            get { return _usernameEdited; }
            set
            {
                _usernameEdited = value;
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

        public int AccountId
        {
            get { return PlayerSingleton.Instance.Player.AccountId; }
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

        public int GamesWon
        {
            get => _gamesWon;
            set { _gamesWon = value; OnPropertyChanged(); }
        }

        public int GamesLost
        {
            get => _gamesLost;
            set { _gamesLost = value; OnPropertyChanged(); }
        }

        public int GamesPlayed
        {
            get => _gamesPlayed;
            set { _gamesPlayed = value; OnPropertyChanged(); }
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

        public bool IsEditUsernameVisible
        {
            get { return _isEditUsernameVisible; }
            set
            {
                _isEditUsernameVisible = value;
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

        public string UsernameError
        {
            get { return _usernameError; }
            set
            {
                _usernameError = value;
                OnPropertyChanged();
            }
        }

        private void ExecuteBackToLobby(Object obj)
        {
            try
            {
                EditProfileTag();
                PlayerSingleton.Instance.Player.LabelPath = PlayerTag;
                _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel)); 
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message); 
            }
        }

        private void EditProfileTag()
        {
            var updatedProfile = new ProfileService.PlayerInfoShownDTO
            {
                Name = Username,
                Id = PlayerId,
                PicturePath = ProfilePicture,
                LabelPath = PlayerTag
            };

            try
            {
                var client = new ProfileModifierServiceClient(new InstanceContext(this));
                client.UpdatePlayerProfileAsync(updatedProfile);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                IsServiceErrorVisible = true;
            }
        }

        private void SelectProfilePicture(Object obj)
        {
            if (obj is string selectedPicture)
            {
                SelectedProfilePicture = selectedPicture;
            }
        }

        private void ToggleProfileSelectorVisibility(Object obj)
        {
            IsProfileSelectorVisible = !IsProfileSelectorVisible;
        }

        private void ToggleEditUsernameVisibility(Object obj)
        {
            IsEditUsernameVisible = !IsEditUsernameVisible;
        }

        private void CancelProfileSelection(Object obj)
        {
            IsProfileSelectorVisible = false;
        }

        private void CancelEditUsername(Object obj)
        {
            IsEditUsernameVisible = false;
        }

        private bool CanAcceptEditUsername(Object obj)
        {
            if (string.IsNullOrEmpty(UsernameEdited))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void AcceptEditUsername(Object obj)
        {
            if (Validations.IsValidUsername(UsernameEdited))
            {
                UsernameError = string.Empty;

                var updatedProfile = new ProfileService.PlayerInfoShownDTO
                {
                    Name = UsernameEdited,
                    Id = PlayerId,
                    PicturePath = ProfilePicture,
                    LabelPath = PlayerTag
                };

                try
                {
                    var client = new ProfileModifierServiceClient(new InstanceContext(this));
                    await client.UpdatePlayerProfileAsync(updatedProfile);
                    PlayerSingleton.Instance.Player.Name = UsernameEdited;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    IsServiceErrorVisible = true;
                }
            }
            else
            {
                UsernameError = Properties.Resources.InvalidUsername_Label;
            }
        }

        private async void ConfirmProfileSelection(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedProfilePicture))
                {
                    var updatedProfile = new ProfileService.PlayerInfoShownDTO
                    {
                        Name = Username,
                        Id = PlayerId,
                        PicturePath = SelectedProfilePicture,
                        LabelPath = PlayerTag
                    };

                    var client = new ProfileModifierServiceClient(new InstanceContext(this));

                    await client.UpdatePlayerProfileAsync(updatedProfile);

                    PlayerSingleton.Instance.Player.PicturePath = SelectedProfilePicture;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la imagen de perfil: " + ex.Message);
            }
        }

        private void Logout(Object obj)
        {
            var client = new ProfileDataServiceClient(new InstanceContext(this));
            client.LogOut(PlayerId);
            _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel));
        }

        private void LoadPlayerInfoFromSingleton()
        {
            if (PlayerSingleton.Instance.IsLoggedIn())
            {
                var player = PlayerSingleton.Instance.Player;
                Username = player.Name;
                PlayerId = player.Id;
                ProfilePicture = player.PicturePath;
                PlayerTag = player.LabelPath;
            }
        }

        private void ExecuteCloseServerError(object obj)
        {
            IsServiceErrorVisible = false;
        }

        public void PlayerInfo(PlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                Username = response.Profile.Name;
                PlayerId = response.Profile.Id;
                ProfilePicture = response.Profile.PicturePath;
            }
        }

        private async void LoadPlayerStatisticsAsync()
        {
            try
            {
                var client = new ProfileDataServiceClient(new InstanceContext(this));
                await client.GetPlayerStatisticsAsync(AccountId);
            }
            catch (Exception ex)
            {
                Log.Error("Error al cargar las estadísticas del jugador", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void PlayerStatistics(PlayerStatisticsResponse response)
        {
            if (response.Result.IsSuccess) 
            { 
                GamesWon = response.Statistics.WonGames;
                GamesLost = response.Statistics.LostGames;
                GamesPlayed = response.Statistics.TotalGames;
            }
        }

        public void ReceiveUpdatePlayerProfile(PlayerInfoResponse result)
        {
            if (result.Result.IsSuccess)
            {
                Username = result.Profile.Name;
                ProfilePicture = result.Profile.PicturePath;

                MessageBox.Show("Perfil actualizado exitosamente en el servidor.");
                if (IsEditUsernameVisible)
                {
                    IsEditUsernameVisible = false;
                }
                else
                {
                    IsProfileSelectorVisible = false;
                }
            }
            else
            {
                MessageBox.Show("No se pudo actualizar el perfil en el servidor.");
                IsEditUsernameVisible = false;
            }
        }
    }
}
