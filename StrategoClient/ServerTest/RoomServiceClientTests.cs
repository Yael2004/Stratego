using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoApp.RoomService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    [TestClass]
    public class RoomServiceClientIntegrationTests
    {
        private ServiceHost _host;
        private RoomServiceClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new RoomServiceClient(new InstanceContext(this));
        }

        [TestCleanup]
        public void TearDown()
        {
            _host?.Close();
            var communicationObject = (ICommunicationObject)_client;
            communicationObject?.Close();
        }

        [TestMethod]
        public async Task ClientCanCallCreateRoom()
        {
            var result = await _client.CreateRoomAsync(1);
            Assert.IsTrue(result);
        }
    }

}
