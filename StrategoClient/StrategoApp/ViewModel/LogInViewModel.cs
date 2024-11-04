﻿using log4net;
using StrategoApp.Helpers;
using StrategoApp.LogInService;
using StrategoApp.Model;
using StrategoApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Converters;

namespace StrategoApp.ViewModel
{
    public class LogInViewModel : ViewModelBase, LogInService.ILogInServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isServiceErrorVisible;
        private bool _isPasswordVisible;
        private bool _isDatabaseError;
        private string _togglePasswordVisibilityIcon;

        public string LogInErrorMessage { get; set; }

        private readonly LogInServiceClient _logInServiceClient;

        private readonly MainWindowViewModel _mainWindowViewModel;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
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

        public bool IsPasswordVisible
        {
            get { return _isPasswordVisible; }
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
                _togglePasswordVisibilityIcon = _isPasswordVisible ? "HidePasswordIcon" : "ShowPasswordIcon";
            }
        }

        public string TogglePasswordVisibilityIcon
        {
            get { return _togglePasswordVisibilityIcon; }
            set
            {
                _togglePasswordVisibilityIcon = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsDatabaseError
        {
            get { return _isDatabaseError; }
            set
            {
                _isDatabaseError = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogInCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand LogInAsInvitedCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }

        public LogInViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _logInServiceClient = new LogInServiceClient(new System.ServiceModel.InstanceContext(this));
            _mainWindowViewModel = mainWindowViewModel;

            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            LogInAsInvitedCommand = new ViewModelCommand(ExcuteLogInAsInvitedCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
            TogglePasswordVisibilityCommand = new ViewModelCommand(p => ExecuteTogglePasswordVisibilityCommand());
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(ExecuteCloseServerError);
            IsServiceErrorVisible = false;
        }

        public LogInViewModel()
        {
            _logInServiceClient = new LogInServiceClient(new System.ServiceModel.InstanceContext(this));

            LogInCommand = new ViewModelCommand(ExecuteLogInCommand, CanExecuteLogInCommand);
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
            LogInAsInvitedCommand = new ViewModelCommand(ExcuteLogInAsInvitedCommand);
            TogglePasswordVisibilityCommand = new ViewModelCommand(p => ExecuteTogglePasswordVisibilityCommand());
        }

        private void ExecuteTogglePasswordVisibilityCommand()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void ExecuteSignUpCommand()
        {
            _mainWindowViewModel.ChangeViewModel(new SignUpViewModel(_mainWindowViewModel));
        }

        private bool CanExecuteLogInCommand(object obj)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                return false;
            }
                
            return true;
        }

        private async void ExecuteLogInCommand(object obj)
        {
            string hashedPassword = HashPassword(Password);
            try
            {
                await _logInServiceClient.LogInAsync(Username, hashedPassword);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                IsServiceErrorVisible = true;
            }
        }

        private void ExcuteLogInAsInvitedCommand(object obj)
        {
            try
            {
                PlayerSingleton.Instance.LogIn(MappingInvited());
                _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                IsServiceErrorVisible = true;
            }
        }

        private void ExecuteCloseServerError(object obj)
        {
            IsServiceErrorVisible = false;
        }

        public void LogInResult(OperationResult result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (result.IsSuccess)
                {
                    _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
                }
                else
                {
                    ErrorMessage = Properties.Resources.NonexistentAccount_Label;
                }
            });
        }

        public void AccountInfo(PlayerDTO player)
        {
            Player playerInstance = MappingPlayer(player);
            PlayerSingleton.Instance.LogIn(playerInstance);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                StringBuilder stringBuilder = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        private Player MappingPlayer(PlayerDTO playerDTO)
        {
            Player player = new Player
            {
                Name = playerDTO.Name,
                Id = playerDTO.Id,
                LabelPath = playerDTO.LabelPath,
                PicturePath = playerDTO.PicturePath
            };

            return player;
        }

        private Player MappingInvited()
        {
            Player player = new Player
            {
                Name = "Invited",
                Id = 0,
                LabelPath = Properties.Resources.Apprentice_Label,
                PicturePath = "pack://application:,,,/Assets/Images/ProfilePictures/Picture1.png",
            };

            return player;
        }
    }
}
