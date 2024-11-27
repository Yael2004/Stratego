﻿using StrategoServices.Data;
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
    public class ProfileService : Interfaces.IProfileDataService, Interfaces.IPlayerFriendsListService, Interfaces.IProfileModifierService, 
        Interfaces.IOtherProfileDataService, Interfaces.ITopPlayersListService
    {
        private readonly Lazy<ProfilesManager> _profilesManager;
        private readonly ConnectedPlayersManager _connectedPlayersManager;

        public ProfileService(Lazy<ProfilesManager> profilesManager, ConnectedPlayersManager connectedPlayersManager)
        {
            _profilesManager = profilesManager;
            _connectedPlayersManager = connectedPlayersManager;
        }

        public async Task GetOtherPlayerInfoAsync(int playerId, int requesterPlayerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IOtherProfileDataCallback>();
            var response = new OtherPlayerInfoResponse();

            try
            {
                var result = _profilesManager.Value.GetPlayerInfo(playerId, requesterPlayerId);

                if (!result.IsSuccess)
                {
                    response.Result = new OperationResult(false, result.Error);
                    response.PlayerInfo = new OtherPlayerInfoDTO();
                }
                else
                {
                    response.PlayerInfo = result.Value;
                    response.Result = new OperationResult(true, "Player info retrieved successfully");
                }
            }
            catch (TimeoutException)
            {
                response.Result = new OperationResult(false, "Server error");
                response.PlayerInfo = new OtherPlayerInfoDTO();
            }
            catch (Exception)
            {
                response.Result = new OperationResult(false, "Unexpected error");
                response.PlayerInfo = new OtherPlayerInfoDTO();
            }
            
            await Task.Run(() => callback.ReceiveOtherPlayerInfo(response));
        }

        public async Task GetPlayerStatisticsAsync(int playerAccountId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IProfileDataServiceCallback>();
            var response = new PlayerStatisticsResponse();

            try
            {
                var result = _profilesManager.Value.GetPlayerGameStatistics(playerAccountId);

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
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IProfileModifierServiceCallback>();
            var response = new PlayerInfoResponse();

            try
            {
                var updateResult = _profilesManager.Value.UpdatePlayerProfile(newProfile);

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

            await Task.Run(() => callback.ReceiveUpdatePlayerProfile(response));
        }

        public async Task GetPlayerFriendsListAsync(int playerId)
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.IPlayerFriendsListServiceCallback>();
            var response = new PlayerFriendsResponse();

            try
            {
                var getFriendsResult = _profilesManager.Value.GetFriendIdsList(playerId);

                if (!getFriendsResult.IsSuccess)
                {
                    response.Result = new OperationResult(false, getFriendsResult.Error);
                }
                else
                {
                    response.Result = new OperationResult(getFriendsResult.IsSuccess, getFriendsResult.Error);
                    response.FriendsIds = getFriendsResult.Value;
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

            await Task.Run(() => callback.PlayerFriendsList(response));
        }

        public async Task GetTopPlayersListAsync()
        {
            var callback = OperationContext.Current.GetCallbackChannel<Interfaces.Callbacks.ITopPlayersListCallback>();
            var response = new TopPlayersResponse();

            try
            {
                var getTopPlayersResult = _profilesManager.Value.GetTopPlayersIds();

                if (!getTopPlayersResult.IsSuccess)
                {
                    response.Result = new OperationResult(false, getTopPlayersResult.Error);
                }
                else
                {
                    response.Result = new OperationResult(getTopPlayersResult.IsSuccess, getTopPlayersResult.Error);
                    response.TopPlayersIds = getTopPlayersResult.Value;
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

            await Task.Run(() => callback.TopPlayersList(response));
        }

        public void LogOut(int playerId)
        {
            if (_connectedPlayersManager.RemovePlayer(playerId))
            {
                Console.WriteLine($"Player {playerId} eliminated from the list");
            }
            else
            {
                Console.WriteLine($"Error: Player {playerId} wasn't in the list");
            }
        }
    }
}
