using log4net;
using StrategoApp.Helpers;
using StrategoApp.Model;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class ScoreboardViewModel : ViewModelBase, ProfileService.ITopPlayersListServiceCallback, ProfileService.IOtherProfileDataServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private string _exceptionMessage;
        private int _userId;
        private bool _isServiceErrorVisible;

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private readonly TopPlayersListServiceClient _topPlayersListServiceClient;

        public ObservableCollection<PlayerScore> _playerScores;
        private readonly PingCheck _pingCheck;

        public ICommand BackToLobbyCommand { get; }
        public ICommand ViewProfileCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }

        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set
            {
                _exceptionMessage = value;
                OnPropertyChanged();
            }
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

        public bool IsServiceErrorVisible
        {
            get { return _isServiceErrorVisible; }
            set
            {
                _isServiceErrorVisible = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlayerScore> PlayerScores
        {
            get { return _playerScores; }
            set
            {
                _playerScores = value;
                OnPropertyChanged();
            }
        }


        public ScoreboardViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _topPlayersListServiceClient = new TopPlayersListServiceClient(new InstanceContext(this));

            _mainWindowViewModel = mainWindowViewModel;

            _pingCheck = new PingCheck(_mainWindowViewModel);
            Task.Run(() => _pingCheck.StartPingMonitoringAsync());

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            ViewProfileCommand = new ViewModelCommand(ViewProfile);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(CloseServiceError);

            LoadTopPlayers();

            _playerScores = new ObservableCollection<PlayerScore>();

            UserId = PlayerSingleton.Instance.Player.Id;
        }

        public void BackToLobby(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
            _pingCheck.StopPingMonitoring();
        }

        public void ViewProfile(object obj)
        {
            if (obj is PlayerScore playerScore)
            {
                try
                {
                    var playerProfileNotOwnViewModel = new PlayerProfileNotOwnViewModel(_mainWindowViewModel);

                    playerProfileNotOwnViewModel.LoadPlayerInfo(playerScore.PlayerId , UserId);
                    _mainWindowViewModel.ChangeViewModel(playerProfileNotOwnViewModel);
                    _pingCheck.StopPingMonitoring();
                }
                catch (CommunicationException cex)
                {
                    Log.Error("Communication error with the connect service.", cex);
                    ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                    IsServiceErrorVisible = true;
                    _pingCheck.StopPingMonitoring();
                }
                catch (TimeoutException tex)
                {
                    Log.Error("Timed out while communicating with the connect service.", tex);
                    ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                    IsServiceErrorVisible = true;
                    _pingCheck.StopPingMonitoring();
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while connecting in.", ex);
                    ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                    IsServiceErrorVisible = true;
                    _pingCheck.StopPingMonitoring();
                }
            }
        }

        private void LoadTopPlayers()
        {
            try
            {
                _topPlayersListServiceClient.GetTopPlayersListAsync();
            }
            catch (CommunicationException cex)
            {
                Log.Error("Communication error while getting top players list.", cex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (TimeoutException tex)
            {
                Log.Error("Timed out while getting top players list.", tex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error while getting top players list.", ex);
                ExceptionMessage = Properties.Resources.ServerConnectionLostMessage_Label;
                IsServiceErrorVisible = true;
                _pingCheck.StopPingMonitoring();
            }
        }

        public void TopPlayersList([MessageParameter(Name = "topPlayersList")] TopPlayersResponse topPlayersList1)
        {
            if (topPlayersList1.Result.IsSuccess)
            {
                foreach (var playerId in topPlayersList1.TopPlayersIds)
                {
                   LoadPlayerInfo(playerId);
                }
            }
            else if (topPlayersList1.Result.IsDataBaseError)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
        }

        private async void LoadPlayerInfo(int playerId)
        {
            await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(playerId, UserId);
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                var playerScore = new PlayerScore
                {
                    PlayerId = response.PlayerInfo.PlayerInfo.Id,
                    PlayerName = response.PlayerInfo.PlayerInfo.Name,
                    Position = response.PlayerInfo.PlayerStatistics.WonGames
                };
                PlayerScores.Add(playerScore);

                var sortedList = PlayerScores.OrderByDescending(p => p.Position).ToList();

                PlayerScores.Clear();
                foreach (var player in sortedList)
                {
                    PlayerScores.Add(player);
                }
            }
            else if (response.Result.IsDataBaseError)
            {
                ExceptionMessage = Properties.Resources.DatabaseConnectionErrorMessage_Label;
                IsServiceErrorVisible = true;
            }
        }

        private void CloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
        }
    }
}
