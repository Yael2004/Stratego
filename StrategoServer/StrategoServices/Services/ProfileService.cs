using log4net;
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
using Utilities;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ProfileService : Interfaces.IProfileDataService, Interfaces.IPlayerFriendsListService, Interfaces.IProfileModifierService, 
        Interfaces.IOtherProfileDataService, Interfaces.ITopPlayersListService
    {
        private readonly Lazy<ProfilesManager> _profilesManager;
        private readonly ConnectedPlayersManager _connectedPlayersManager;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProfileService));

        public ProfileService(Lazy<ProfilesManager> profilesManager, ConnectedPlayersManager connectedPlayersManager)
        {
            _profilesManager = profilesManager;
            _connectedPlayersManager = connectedPlayersManager;
        }

        /// <summary>
        /// Obtains the player's profile information
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="requesterPlayerId"></param>
        /// <returns>Task</returns>
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
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                response.Result = new OperationResult(false, Messages.TimeoutError);
                response.PlayerInfo = new OtherPlayerInfoDTO();
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                response.Result = new OperationResult(false, Messages.UnexpectedError);
                response.PlayerInfo = new OtherPlayerInfoDTO();
            }
            
            await Task.Run(() => callback.ReceiveOtherPlayerInfo(response));
        }

        /// <summary>
        /// Obtains the player's statistics
        /// </summary>
        /// <param name="playerAccountId"></param>
        /// <returns>Task</returns>
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
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                response.Result = new OperationResult(false, Messages.TimeoutError);
                response.Statistics = new PlayerStatisticsDTO();
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                response.Result = new OperationResult(false, Messages.UnexpectedError);
                response.Statistics = new PlayerStatisticsDTO();
            }

            await Task.Run(() => callback.PlayerStatistics(response));
        }

        /// <summary>
        /// Updates the player's profile information
        /// </summary>
        /// <param name="newProfile"></param>
        /// <returns>Task</returns>
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
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                response.Result = new OperationResult(false, Messages.TimeoutError);
                response.Profile = new PlayerInfoShownDTO();
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                response.Result = new OperationResult(false, $"{Messages.UnexpectedError}: {ex.Message}");
                response.Profile = new PlayerInfoShownDTO();
            }

            await Task.Run(() => callback.ReceiveUpdatePlayerProfile(response));
        }

        /// <summary>
        /// Obtains the player's friends ids list
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>Task</returns>
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
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                response.Result = new OperationResult(false, Messages.TimeoutError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                response.Result = new OperationResult(false, $"{Messages.UnexpectedError}: {ex.Message}");
            }

            await Task.Run(() => callback.PlayerFriendsList(response));
        }

        /// <summary>
        /// Obtains the top global players ids list
        /// </summary>
        /// <returns>Task</returns>
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
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                response.Result = new OperationResult(false, Messages.TimeoutError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                response.Result = new OperationResult(false, $"{Messages.UnexpectedError} : {ex.Message}");
            }

            await Task.Run(() => callback.TopPlayersList(response));
        }

        /// <summary>
        /// Obtains the refined connected friends ids list
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>Task</returns>
        public async Task GetConnectedFriendsAsync(int playerId)
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
                    var connectedFriends = getFriendsResult.Value
                        .Where(friendId => _connectedPlayersManager.IsPlayerConnected(friendId))
                        .ToList();

                    response.Result = new OperationResult(true, "Connected friends retrieved successfully");
                    response.FriendsIds = connectedFriends;
                }
            }
            catch (TimeoutException tex)
            {
                log.Error(Messages.TimeoutError, tex);
                response.Result = new OperationResult(false, Messages.TimeoutError);
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                response.Result = new OperationResult(false, $"{Messages.UnexpectedError} : {ex.Message}");
            }

            await Task.Run(() => callback.PlayerFriendsList(response));
        }

        /// <summary>
        /// Remove a player from the connected players list
        /// </summary>
        /// <param name="playerId"></param>
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
