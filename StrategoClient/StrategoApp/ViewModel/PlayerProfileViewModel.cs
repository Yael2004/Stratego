﻿using StrategoApp.Model;
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
        private string _profilePicture;
        private string _playerTag;
        private string _selectedProfilePicture;
        private string _usernameError;
        private string _updateProfileResult;
        private bool _isServiceErrorVisible;
        private bool _isProileSelectorVisible;
        private bool _isEditUsernameVisible;
        private bool _isUpdateResultVisible;
        private int _playerId;
        private int _gamesWon;
        private int _gamesLost;
        private int _gamesPlayed;

        private readonly ProfileModifierServiceClient _playerModifierServiceClient;
        private readonly ProfileDataServiceClient _playerDataServiceClient;

        private readonly MainWindowViewModel _mainWindowViewModel;

        public ObservableCollection<string> ProfilePictures { get; set; }
        public ObservableCollection<string> ProfileTags { get; set; }

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

        public string UpdateProfileResult
        {
            get { return _updateProfileResult; }
            set
            {
                _updateProfileResult = value;
                OnPropertyChanged();
            }
        }

        public static int AccountId
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

        public bool IsUpdateResultVisible
        {
            get { return _isUpdateResultVisible; }
            set
            {
                _isUpdateResultVisible = value;
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

        public PlayerProfileViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _playerModifierServiceClient = new ProfileModifierServiceClient(new InstanceContext(this));
            _playerDataServiceClient = new ProfileDataServiceClient(new InstanceContext(this));

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

            IsServiceErrorVisible = false;

            LoadPlayerInfoFromSingleton();
            LoadPlayerStatisticsAsync();
        }

        private void ExecuteBackToLobby(Object obj)
        {
            EditProfileTag();

            PlayerSingleton.Instance.Player.LabelPath = PlayerTag;

            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel)); 
        }

        private void EditProfileTag()
        {
            switch (PlayerTag)
            {
                case var tag when tag == Properties.Resources.NovicePlayer_Label:
                    PlayerTag = "Label1";
                    break;

                case var tag when tag == Properties.Resources.ProPlayer_Label:
                    PlayerTag = "Label2";
                    break;

                case var tag when tag == Properties.Resources.Apprentice_Label:
                    PlayerTag = "Label3";
                    break;

                case var tag when tag == Properties.Resources.Competitive_Label:
                    PlayerTag = "Label4";
                    break;

                default:
                    PlayerTag = "Label1";
                    break;
            }

            var updatedProfile = new ProfileService.PlayerInfoShownDTO
            {
                Name = Username,
                Id = PlayerId,
                PicturePath = ProfilePicture,
                LabelPath = PlayerTag
            };

            try
            {
                _playerModifierServiceClient.UpdatePlayerProfileAsync(updatedProfile);
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while updating tag.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with the update tag.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while updating tag.", ex);
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
            UsernameEdited = UsernameEdited.Trim();

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
                    await _playerModifierServiceClient.UpdatePlayerProfileAsync(updatedProfile);

                    PlayerSingleton.Instance.Player.Name = UsernameEdited;
                }
                catch (CommunicationException ex)
                {
                    Log.Error("Communication error while updating username.", ex);
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException ex)
                {
                    Log.Error("Timed out while communicating with the update username.", ex);
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while updating username.", ex);
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

                    await _playerModifierServiceClient.UpdatePlayerProfileAsync(updatedProfile);

                    PlayerSingleton.Instance.Player.PicturePath = SelectedProfilePicture;
                }
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while updating profile picture.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with the profile picture update.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while updating profile picture.", ex);
                IsServiceErrorVisible = true;
            }
        }

        private void Logout(Object obj)
        {

            _playerDataServiceClient.LogOut(PlayerId);
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
                
                switch(player.LabelPath)
                {
                    case "Label1":
                        PlayerTag = Properties.Resources.NovicePlayer_Label;
                        break;

                    case "Label2":
                        PlayerTag = Properties.Resources.ProPlayer_Label;
                        break;

                    case "Label3":
                        PlayerTag = Properties.Resources.Apprentice_Label;
                        break;

                    case "Label4":
                        PlayerTag = Properties.Resources.Competitive_Label;
                        break;

                    default:
                        PlayerTag = Properties.Resources.NovicePlayer_Label;
                        break;
                }
            }
        }

        private void ExecuteCloseServerError(object obj)
        {
            IsServiceErrorVisible = false;
        }

        public void PlayerInfo(PlayerInfoResponse playerInfo1)
        {
            if (playerInfo1.Result.IsSuccess)
            {
                Username = playerInfo1.Profile.Name;
                PlayerId = playerInfo1.Profile.Id;
                ProfilePicture = playerInfo1.Profile.PicturePath;
            }
        }

        private async void LoadPlayerStatisticsAsync()
        {
            try
            {
                await _playerDataServiceClient.GetPlayerStatisticsAsync(AccountId);
            }
            catch (CommunicationException ex)
            {
                Log.Error("Communication error while getting player statistics.", ex);
                IsServiceErrorVisible = true;
            }
            catch (TimeoutException ex)
            {
                Log.Error("Timed out while communicating with the get player statistics.", ex);
                IsServiceErrorVisible = true;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while getting player statistics.", ex);
                IsServiceErrorVisible = true;
            }
        }

        public void PlayerStatistics(PlayerStatisticsResponse playerStatistics1)
        {
            if (playerStatistics1.Result.IsSuccess) 
            { 
                GamesWon = playerStatistics1.Statistics.WonGames;
                GamesLost = playerStatistics1.Statistics.LostGames;
                GamesPlayed = playerStatistics1.Statistics.TotalGames;
            }
        }

        public void ReceiveUpdatePlayerProfile(PlayerInfoResponse result)
        {
            if (result.Result.IsSuccess)
            {
                Username = result.Profile.Name;
                ProfilePicture = result.Profile.PicturePath;

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
