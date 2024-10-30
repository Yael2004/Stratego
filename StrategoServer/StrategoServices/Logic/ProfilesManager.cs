using StrategoDataAccess;
using StrategoServices.Data.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /*
        public async Task<Result<PlayerInfoShownDTO>> GetPlayerInfoAsync(int playerId)
        {

        }
        */

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

        public async Task<Result<List<PlayerInfoShownDTO>>> GetFriendsListAsync(int playerId)
        {
            var friendsResult = await GetFriendsFromRepositoryAsync(playerId);

            if (!friendsResult.IsSuccess)
            {
                return Result<List<PlayerInfoShownDTO>>.Failure(friendsResult.Error);
            }

            var friendsDtoList = new List<PlayerInfoShownDTO>();
            foreach (var friend in friendsResult.Value)
            {
                var friendDtoResult = await MapPlayerToPlayerInfoShownDTOAsync(friend);
                if (!friendDtoResult.IsSuccess)
                {
                    return Result<List<PlayerInfoShownDTO>>.Failure(friendDtoResult.Error);
                }
                friendsDtoList.Add(friendDtoResult.Value);
            }

            return Result<List<PlayerInfoShownDTO>>.Success(friendsDtoList);
        }

        private async Task<Result<IEnumerable<Player>>> GetFriendsFromRepositoryAsync(int playerId)
        {
            return await _playerRepository.Value.GetPlayerFriendsListAsync(playerId);
        }

        private async Task<Result<PlayerInfoShownDTO>> MapPlayerToPlayerInfoShownDTOAsync(Player player)
        {
            try
            {
                var picturePathResult = await _playerRepository.Value.GetPicturePathByIdAsync(player.PictureId ?? 1);
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
