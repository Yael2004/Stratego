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

        public async Task UpdatePlayerProfileAsync(PlayerInfoShownDTO newProfile)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IProfileServiceCallback>();
            var response = new PlayerInfoResponse();

            try
            {
                var updateResult = await _profilesManager.Value.UpdatePlayerProfileAsync(newProfile);

                if (!updateResult.IsSuccess)
                {
                    response.Result = new OperationResult(false, updateResult.Error);
                    response.Profile = new PlayerInfoShownDTO();
                }
                else
                {
                    response.Result = new OperationResult(updateResult.IsSuccess, updateResult.IsSuccess ? "Profile updated successfully" : updateResult.Error);
                    response.Profile = new PlayerInfoShownDTO
                    {
                        Id = newProfile.Id,
                        Name = newProfile.Name,
                        PicturePath = newProfile.PicturePath,
                        LabelPath = newProfile.LabelPath
                    };
                }
            }
            catch (TimeoutException)
            {
                response.Result = new OperationResult(false, "Server error");
                response.Profile = new PlayerInfoShownDTO();
            }
            catch (Exception ex)
            {
                response.Result = new OperationResult(false, $"Unexpected error: {ex.Message}");
                response.Profile = new PlayerInfoShownDTO();
            }

            callback.ReceiveUpdatePlayerProfile(response);
        }

        public async Task GetPlayerFriendsListAsync(int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IProfileServiceCallback>();
            var response = new PlayerFriendsResponse();

            try
            {
                var getFriendsResult = await _profilesManager.Value.GetFriendsListAsync(playerId);

                if (!getFriendsResult.IsSuccess)
                {
                    response.Result = new OperationResult(false, getFriendsResult.Error);
                }
                else
                {
                    response.Result = new OperationResult(getFriendsResult.IsSuccess, getFriendsResult.Error);
                    response.Friends = getFriendsResult.Value;
                }
            }
            catch (TimeoutException)
            {
                response.Result = new OperationResult(false, "Server error");
            }
            catch (Exception ex)
            {
                response.Result = new OperationResult(false, $"Unexpected error: {ex.Message}");
            }

            callback.PlayerFriendsList(response);
        }

    }
}
