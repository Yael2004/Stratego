using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerTest.PingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest.Server
{
    [TestClass]
    public class PingServiceClientTests
    {
        private PingServiceClient _client;

        [TestInitialize]
        public void Setup()
        {
            _client = new PingServiceClient("NetTcpBinding_IPingService");
        }

        [TestMethod]
        public void Test_Ping_Success()
        {
            var response = _client.Ping();
            Assert.IsTrue(response);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_client.State == CommunicationState.Opened)
            {
                _client.Close();
            }
        }
    }
}
