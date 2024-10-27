using StrategoDataAccess;
using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ProfileService : Interfaces.IProfileService
    {
        private readonly Lazy<ProfilesManager> _profilesManager;

        public ProfileService(Lazy<ProfilesManager> profilesManager)
        {
            _profilesManager = profilesManager;
        }

        public Task GetPlayerInfoAsync(int playerId)
        {
            throw new NotImplementedException();
        }

        public async Task GetPlayerStatisticsAsync(int playerAccountId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IProfileServiceCallback>();
            var response = new PlayerStatisticsResponse();

            try
            {
                var result = await _profilesManager.Value.GetPlayerGameStatisticsAsync(playerAccountId);

                if (!result.IsSuccess)
                {
                    response.Result = new OperationResult(false, result.Error);
                    response.Statistics = new PlayerStatisticsDTO();
                }
                else
                {
                    response.Statistics = new PlayerStatisticsDTO
                    {
                        WonGames = result.Value.WonGames,
                        LostGames = result.Value.LostGames
                    };
                    response.Result = new OperationResult(true, "Player statistics retrieved successfully");
                }
            }
            catch (TimeoutException)
            {
                response.Result = new OperationResult(false, "Server error");
                response.Statistics = new PlayerStatisticsDTO();
            }
            catch (Exception)
            {
                response.Result = new OperationResult(false, "Unexpected error");
                response.Statistics = new PlayerStatisticsDTO();
            }

            await Task.Run(() => callback.PlayerStatistics(response));
        }


        public Task<OperationResult> UpdatePlayerProfileAsync(PlayerInfoShownDTO profile)
        {
            throw new NotImplementedException();
        }
    }
}
