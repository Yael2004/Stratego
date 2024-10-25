using StrategoDataAccess;
using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;

namespace StrategoServices.Services
{
    public class ProfileService : Interfaces.IProfileService
    {
        private readonly Lazy<ProfilesManager> _profilesManager;

        public ProfileService(Lazy<ProfilesManager> profilesManager)
        {
            _profilesManager = profilesManager;
        }

        public Task<PlayerInfoResponse> GetPlayerInfoAsync(int playerId)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerStatisticsResponse> GetPlayerStatisticsAsync(int playerAccountId)
        {
            try
            {
                var result = await _profilesManager.Value.GetPlayerGameStatisticsAsync(playerAccountId);
                var response = new PlayerStatisticsResponse();

                if (!result.IsSuccess)
                {
                    response.Result = new OperationResult(false, result.Error);
                    response.Statistics = new PlayerStatisticsDTO();
                    return response;
                }

                response.Statistics = new PlayerStatisticsDTO
                {
                    WonGames = result.Value.WonGames,
                    LostGames = result.Value.LostGames
                };

                response.Result = new OperationResult(true, "Player statistics retrieved successfully");

                return response;
            }
            catch (TimeoutException)
            {
                return new PlayerStatisticsResponse
                {
                    Result = new OperationResult(false, "Server error"),
                    Statistics = new PlayerStatisticsDTO()
                };
            }
            catch (Exception)
            {
                return new PlayerStatisticsResponse
                {
                    Result = new OperationResult(false, "Unexpected error"),
                    Statistics = new PlayerStatisticsDTO()
                };
            }
        }

        public Task<OperationResult> UpdatePlayerProfileAsync(PlayerInfoShownDTO profile)
        {
            throw new NotImplementedException();
        }
    }
}
