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

        public ObservableCollection<PlayerScore> PlayerScores { get; set; }

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


        public ScoreboardViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _topPlayersListServiceClient = new TopPlayersListServiceClient(new InstanceContext(this));

            _mainWindowViewModel = mainWindowViewModel;

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            ViewProfileCommand = new ViewModelCommand(ViewProfile);
            ExecuteCloseServiceErrorCommand = new ViewModelCommand(CloseServiceError);

            LoadTopPlayers();

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

                    playerProfileNotOwnViewModel.LoadPlayerInfo(playerScore.PlayerId ,UserId);
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

        public async void TopPlayersList([MessageParameter(Name = "topPlayersList")] TopPlayersResponse topPlayersList1)
        {
            PlayerScores = new ObservableCollection<PlayerScore>();

            foreach (var playerId in topPlayersList1.TopPlayersIds)
            {
                await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(playerId, UserId);
            }

            OnPropertyChanged(nameof(PlayerScores));
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

                OnPropertyChanged(nameof(PlayerScores));
            }
        }

        private void CloseServiceError(object obj)
        {
            IsServiceErrorVisible = false;
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
        }
    }
}
