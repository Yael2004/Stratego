using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.ProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    public class TestProfileDataServiceCallback : IProfileDataServiceCallback
    {
        public PlayerStatisticsResponse PlayerStatisticsResponse { get; private set; }
        public PlayerInfoResponse PlayerInfoResponse { get; private set; }

        public void PlayerStatistics(PlayerStatisticsResponse playerStatistics)
        {
            PlayerStatisticsResponse = playerStatistics;
        }

        public void PlayerInfo(PlayerInfoResponse playerInfo)
        {
            PlayerInfoResponse = playerInfo;
        }
    }

    [TestClass]
    public class ProfileServiceClientTest
    {
        private TestProfileDataServiceCallback _callback;
        private ProfileDataServiceClient _client;

        [TestInitialize]
        public void SetUp() 
        { 
            _callback = new TestProfileDataServiceCallback();
            var instanceContext = new InstanceContext(_callback);
            _client = new ProfileDataServiceClient(instanceContext);
        }

        [TestMethod]
        public async Task GetPlayerStatistics_ShouldReturnStatistics()
        {
            await _client.GetPlayerStatisticsAsync(1017);

            Assert.AreEqual(_callback.PlayerStatisticsResponse.Statistics.WonGames, 3);
        }
    }
}
