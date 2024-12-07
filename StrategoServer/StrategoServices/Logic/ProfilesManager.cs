using StrategoDataAccess;
using StrategoServices.Data.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;
using Utilities;

namespace StrategoServices.Logic
{
    public class ProfilesManager
    {
        private readonly Lazy<GamesRepository> _gamesRepository;
        private readonly Lazy<PlayerRepository> _playerRepository;

        public ProfilesManager(Lazy<GamesRepository> gamesRepository, Lazy<PlayerRepository> playerRepository)
        {
            _gamesRepository = gamesRepository;
            _playerRepository = playerRepository;
        }

        public Result<PlayerStatisticsDTO> GetPlayerGameStatistics(int accountId)
        {
            var result = _gamesRepository.Value.GetGameStatisticsByAccountId(accountId);

            if (!result.IsSuccess)
            {
                return Result<PlayerStatisticsDTO>.Failure(result.Error);
            }

            var gameStatsDto = new PlayerStatisticsDTO
            {
                WonGames = result.Value.WonGames,
                LostGames = result.Value.DeafeatGames,
            };

            return Result<PlayerStatisticsDTO>.Success(gameStatsDto);
        }

        public Result<OtherPlayerInfoDTO> GetPlayerInfo(int playerId, int requesterId)
        {
            try
            {
                var playerResult = _playerRepository.Value.GetOtherPlayerById(playerId);
                if (!playerResult.IsSuccess)
                {
                    return Result<OtherPlayerInfoDTO>.Failure(playerResult.Error);
                }

                var picturePath = _playerRepository.Value.GetPicturePathById(playerResult.Value.PictureId);
                var labelPath = _playerRepository.Value.GetLabelPathById(playerResult.Value.IdLabel);
                
                var playerInfoDto = new PlayerInfoShownDTO
                {
                    Id = playerResult.Value.Id,
                    Name = playerResult.Value.Name,
                    PicturePath = picturePath.Value,
                    LabelPath = labelPath.Value
                };

                var playerStatisticsResult = _gamesRepository.Value.GetGameStatisticsByAccountId(playerResult.Value.AccountId);
                if (!playerStatisticsResult.IsSuccess)
                {
                    return Result<OtherPlayerInfoDTO>.Failure(playerStatisticsResult.Error);
                }

                var isFriendResult = _playerRepository.Value.IsFriend(requesterId, playerId);
                if (!isFriendResult.IsSuccess)
                {
                    return Result<OtherPlayerInfoDTO>.Failure(isFriendResult.Error);
                }

                var playerStatistics = new PlayerStatisticsDTO
                {
                    WonGames = playerStatisticsResult.Value.WonGames,
                    LostGames = playerStatisticsResult.Value.DeafeatGames
                };

                var response = new OtherPlayerInfoDTO
                {
                    PlayerInfo = playerInfoDto,
                    PlayerStatistics = playerStatistics,
                    IsFriend = isFriendResult.Value
                };

                return Result<OtherPlayerInfoDTO>.Success(response);
            }
            catch (SqlException sqlEx)
            {
                return Result<OtherPlayerInfoDTO>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<OtherPlayerInfoDTO>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public Result<PlayerInfoShownDTO> UpdatePlayerProfile(PlayerInfoShownDTO PlayerInfoShownDTO)
        {
            var player = MapPlayerInfoShownDTOToPlayer(PlayerInfoShownDTO);

            var result = _playerRepository.Value.UpdatePlayer(player, PlayerInfoShownDTO.LabelPath, PlayerInfoShownDTO.PicturePath);

            if (!result.IsSuccess)
            {
                return Result<PlayerInfoShownDTO>.Failure(result.Error);
            }

            var updatedPlayerInfoShownDTO = MapPlayerToPlayerInfoShownDTO(result.Value, PlayerInfoShownDTO.LabelPath, PlayerInfoShownDTO.PicturePath);

            return Result<PlayerInfoShownDTO>.Success(updatedPlayerInfoShownDTO);
        }

        private Player MapPlayerInfoShownDTOToPlayer(PlayerInfoShownDTO PlayerInfoShownDTO)
        {
            return new Player
            {
                Id = PlayerInfoShownDTO.Id,
                Name = PlayerInfoShownDTO.Name,
                PictureId = 0,
                IdLabel = 0
            };
        }

        private PlayerInfoShownDTO MapPlayerToPlayerInfoShownDTO(Player player, string labelPath, string picturePath)
        {
            return new PlayerInfoShownDTO
            {
                Id = player.Id,
                Name = player.Name,
                LabelPath = labelPath,
                PicturePath = picturePath
            };
        }

        public Result<List<int>> GetFriendIdsList(int playerId)
        {
            var friendsResult = GetFriendsFromRepository(playerId);

            if (!friendsResult.IsSuccess)
            {
                return Result<List<int>>.Failure(friendsResult.Error);
            }

            var friendIds = friendsResult.Value.Select(friend => friend.Id).ToList();

            return friendIds.Any()
                ? Result<List<int>>.Success(friendIds)
                : Result<List<int>>.Failure("No friends found.");
        }

        private Result<IEnumerable<Player>> GetFriendsFromRepository(int playerId)
        {
            return _playerRepository.Value.GetPlayerFriendsList(playerId);
        }

        public Result<List<int>> GetTopPlayersIds()
        {
            var topPlayersResult = GetTopPlayersFromRepository();

            if (!topPlayersResult.IsSuccess)
            {
                return Result<List<int>>.Failure(topPlayersResult.Error);
            }
            return Result<List<int>>.Success(topPlayersResult.Value);
        }

        private Result<List<int>> GetTopPlayersFromRepository()
        {
            return _playerRepository.Value.GetTopPlayersByWins();
        }
    }
}
