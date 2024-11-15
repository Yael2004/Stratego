using StrategoApp.Model;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    public class ScoreboardViewModel : ViewModelBase, ProfileService.ITopPlayersListServiceCallback, ProfileService.IOtherProfileDataServiceCallback
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private TopPlayersListServiceClient _topPlayersListServiceClient;
        private int _userId;
        private string _username;
        private int _score;

        public ObservableCollection<PlayerScore> PlayerScores { get; set; }

        public ICommand BackToLobbyCommand { get; }

        public ScoreboardViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _topPlayersListServiceClient = new TopPlayersListServiceClient(new InstanceContext(this));
            _mainWindowViewModel = mainWindowViewModel;

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            LoadTopPlayers();
            _userId = PlayerSingleton.Instance.Player.Id;
        }

        public void BackToLobby(object obj)
        {
            _mainWindowViewModel.ChangeViewModel(new LobbyViewModel(_mainWindowViewModel));
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
                await _otherProfileDataServiceClient.GetOtherPlayerInfoAsync(playerId, _userId);
            }

            OnPropertyChanged(nameof(PlayerScores));
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            if (response.Result.IsSuccess)
            {
                var playerScore = new PlayerScore
                {
                    PlayerName = response.PlayerInfo.PlayerInfo.Name,
                    Position = response.PlayerInfo.PlayerStatistics.WonGames
                };
                PlayerScores.Add(playerScore);

                OnPropertyChanged(nameof(PlayerScores));
            }
        }
    }
}
