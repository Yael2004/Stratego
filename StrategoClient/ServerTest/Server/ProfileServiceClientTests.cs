using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest.Server
{
    [TestClass]
    public class ProfileServiceClientTests : IProfileDataServiceCallback, IPlayerFriendsListServiceCallback, IProfileModifierServiceCallback,
        IOtherProfileDataServiceCallback, ITopPlayersListServiceCallback
    {
        private ProfileDataServiceClient _profileDataClient;
        private PlayerFriendsListServiceClient _playerFriendsListClient;
        private ProfileModifierServiceClient _profileModifierClient;
        private OtherProfileDataServiceClient _otherProfileDataClient;
        private TopPlayersListServiceClient _topPlayersListClient;

        private PlayerInfoResponse _playerInfoResponse;
        private PlayerFriendsResponse _playerFriendsResponse;
        private PlayerStatisticsResponse _playerStatisticsResponse;
        private OtherPlayerInfoResponse _otherPlayerInfoResponse;
        private PlayerInfoResponse _updateProfileResponse;
        private TopPlayersResponse _topPlayersResponse;

        [TestInitialize]
        public void Setup()
        {
            _profileDataClient = new ProfileDataServiceClient(new InstanceContext(this));
            _playerFriendsListClient = new PlayerFriendsListServiceClient(new InstanceContext(this));
            _profileModifierClient = new ProfileModifierServiceClient(new InstanceContext(this));
            _otherProfileDataClient = new OtherProfileDataServiceClient(new InstanceContext(this));
            _topPlayersListClient = new TopPlayersListServiceClient(new InstanceContext(this));
        }

        [TestMethod]
        public async Task Test_GetPlayerInfo_Success()
        {
            await _otherProfileDataClient.GetOtherPlayerInfoAsync(1, 2);

            Assert.AreEqual("Saay", _otherPlayerInfoResponse.PlayerInfo.PlayerInfo.Name);
        }

        [TestMethod]
        public async Task Test_GetPlayerStatistics_Succes()
        {
            await _profileDataClient.GetPlayerStatisticsAsync(1);

            Assert.IsTrue(_playerStatisticsResponse.Result.IsSuccess);
        }

        [TestMethod]
        public async Task Test_UpdatePlayerProfile_Success()
        {
            var newProfile = new PlayerInfoShownDTO
            {
                Id = 1,
                Name = "Saay",
                PicturePath = "pack://application:,,,/Assets/Images/ProfilePictures/Picture1.png",
                LabelPath = "label1",
            };

            await _profileModifierClient.UpdatePlayerProfileAsync(newProfile);

            Assert.IsTrue(_updateProfileResponse.Result.IsSuccess);
        }

        [TestMethod]
        public async Task Test_GetPlayerFriendsList_Success()
        {
            await _playerFriendsListClient.GetPlayerFriendsListAsync(3);

            Assert.IsTrue(_playerFriendsResponse.Result.IsSuccess);
        }

        [TestMethod]
        public async Task Test_GetTopPlayersList_Success()
        {
            await _topPlayersListClient.GetTopPlayersListAsync();

            Assert.IsTrue(_topPlayersResponse.Result.IsSuccess);
        }

        public void PlayerFriendsList(PlayerFriendsResponse playerFriends)
        {
            _playerFriendsResponse = playerFriends;
        }

        public void PlayerInfo([MessageParameter(Name = "playerInfo")] PlayerInfoResponse playerInfo1)
        {
            _playerInfoResponse = playerInfo1;
        }

        public void PlayerStatistics([MessageParameter(Name = "playerStatistics")] PlayerStatisticsResponse playerStatistics1)
        {
            _playerStatisticsResponse = playerStatistics1;
        }

        public void ReceiveOtherPlayerInfo(OtherPlayerInfoResponse response)
        {
            _otherPlayerInfoResponse = response;
        }

        public void ReceiveUpdatePlayerProfile(PlayerInfoResponse result)
        {
            _updateProfileResponse = result;
        }

        public void TopPlayersList([MessageParameter(Name = "topPlayersList")] TopPlayersResponse topPlayersList1)
        {
            _topPlayersResponse = topPlayersList1;
        }
    }
}
