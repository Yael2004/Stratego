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

        public async Task<Result<PlayerStatisticsDTO>> GetPlayerGameStatisticsAsync(int accountId)
        {
            var result = await _gamesRepository.Value.GetGameStatisticsByAccountIdAsync(accountId);

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

        public async Task<Result<OtherPlayerInfoDTO>> GetPlayerInfoAsync(int playerId, int requesterId)
        {
            try
            {
                var playerResult = await _playerRepository.Value.GetOtherPlayerByIdAsync(playerId);
                if (!playerResult.IsSuccess)
                {
                    return Result<OtherPlayerInfoDTO>.Failure(playerResult.Error);
                }

                var picturePath = await _playerRepository.Value.GetPicturePathByIdAsync(playerResult.Value.PictureId);
                var labelPath = await _playerRepository.Value.GetLabelPathByIdAsync(playerResult.Value.IdLabel);
                
                var playerInfoDto = new PlayerInfoShownDTO
                {
                    Name = playerResult.Value.Name,
                    PicturePath = picturePath.Value,
                    LabelPath = labelPath.Value
                };

                var playerStatisticsResult = await _gamesRepository.Value.GetGameStatisticsByAccountIdAsync(playerResult.Value.AccountId);
                if (!playerStatisticsResult.IsSuccess)
                {
                    return Result<OtherPlayerInfoDTO>.Failure(playerStatisticsResult.Error);
                }

                var isFriendResult = await _playerRepository.Value.IsFriendAsync(requesterId, playerId);
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

        public async Task<Result<PlayerInfoShownDTO>> UpdatePlayerProfileAsync(PlayerInfoShownDTO PlayerInfoShownDTO)
        {
            var player = MapPlayerInfoShownDTOToPlayer(PlayerInfoShownDTO);

            var result = await _playerRepository.Value.UpdatePlayerAsync(player, PlayerInfoShownDTO.LabelPath, PlayerInfoShownDTO.PicturePath);

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

        public async Task<Result<List<int>>> GetFriendIdsListAsync(int playerId)
        {
            var friendsResult = await GetFriendsFromRepositoryAsync(playerId);

            if (!friendsResult.IsSuccess)
            {
                return Result<List<int>>.Failure(friendsResult.Error);
            }

            var friendIds = friendsResult.Value.Select(friend => friend.Id).ToList();

            return friendIds.Any()
                ? Result<List<int>>.Success(friendIds)
                : Result<List<int>>.Failure("No friends found.");
        }

        private async Task<Result<IEnumerable<Player>>> GetFriendsFromRepositoryAsync(int playerId)
        {
            return await _playerRepository.Value.GetPlayerFriendsListAsync(playerId);
        }

        private async Task<Result<PlayerInfoShownDTO>> MapPlayerToPlayerInfoShownDTOAsync(Player player)
        {
            try
            {
                var picturePathResult = await _playerRepository.Value.GetPicturePathByIdAsync(player.PictureId);
                if (!picturePathResult.IsSuccess)
                {
                    return Result<PlayerInfoShownDTO>.Failure("Failed to retrieve picture path.");
                }

                var labelPathResult = await _playerRepository.Value.GetLabelPathByIdAsync(player.IdLabel);
                if (!labelPathResult.IsSuccess)
                {
                    return Result<PlayerInfoShownDTO>.Failure("Failed to retrieve label path.");
                }

                var playerInfoDto = new PlayerInfoShownDTO
                {
                    Name = player.Name,
                    PicturePath = picturePathResult.Value,
                    LabelPath = labelPathResult.Value
                };

                return Result<PlayerInfoShownDTO>.Success(playerInfoDto);
            }
            catch (Exception ex)
            {
                return Result<PlayerInfoShownDTO>.Failure($"Unexpected error: {ex.Message}");
            }
        }

    }
}
