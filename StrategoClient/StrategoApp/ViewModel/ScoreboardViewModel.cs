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
        private readonly MainWindowViewModel _mainWindowViewModel;
        private OtherProfileDataServiceClient _otherProfileDataServiceClient;
        private TopPlayersListServiceClient _topPlayersListServiceClient;
        private int _userId;
        private string _username;
        private int _score;

        public ObservableCollection<PlayerScore> PlayerScores { get; set; }

        public ICommand BackToLobbyCommand { get; }
        public ICommand ViewProfileCommand { get; }

        public ScoreboardViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _otherProfileDataServiceClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _topPlayersListServiceClient = new TopPlayersListServiceClient(new InstanceContext(this));
            _mainWindowViewModel = mainWindowViewModel;

            BackToLobbyCommand = new ViewModelCommand(BackToLobby);
            ViewProfileCommand = new ViewModelCommand(ViewProfile);
            LoadTopPlayers();
            _userId = PlayerSingleton.Instance.Player.Id;
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

                    playerProfileNotOwnViewModel.LoadPlayerInfo(playerScore.PlayerId ,_userId);
                    _mainWindowViewModel.ChangeViewModel(playerProfileNotOwnViewModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar perfil de jugador: " + ex.Message);
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
                    PlayerId = response.PlayerInfo.PlayerInfo.Id,
                    PlayerName = response.PlayerInfo.PlayerInfo.Name,
                    Position = response.PlayerInfo.PlayerStatistics.WonGames
                };
                PlayerScores.Add(playerScore);

                OnPropertyChanged(nameof(PlayerScores));
            }
        }
    }
}
