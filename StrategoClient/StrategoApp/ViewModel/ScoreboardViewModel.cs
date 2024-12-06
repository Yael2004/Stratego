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

        private int _userId;
        private bool _isServiceErrorVisible;

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private readonly TopPlayersListServiceClient _topPlayersListServiceClient;

        public ObservableCollection<PlayerScore> _playerScores;

        public ICommand BackToLobbyCommand { get; }
        public ICommand ViewProfileCommand { get; }
        public ICommand ExecuteCloseServiceErrorCommand { get; }

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
                }
                catch (CommunicationException cex)
                {
                    Log.Error("Communication error with the connect service.", cex);
                    IsServiceErrorVisible = true;
                }
                catch (TimeoutException tex)
                {
                    Log.Error("Timed out while communicating with the connect service.", tex);
                    IsServiceErrorVisible = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Unexpected error while connecting in.", ex);
                    IsServiceErrorVisible = true;
                }
            }
        }

        private void LoadTopPlayers()
        {
            _topPlayersListServiceClient.GetTopPlayersListAsync();
        }

        public void TopPlayersList([MessageParameter(Name = "topPlayersList")] TopPlayersResponse topPlayersList1)
        {
            foreach (var playerId in topPlayersList1.TopPlayersIds)
            {
               LoadPlayerInfo(playerId);
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
        }

        private void CloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }
    }
}
