using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.LogInService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    public class TestLogInServiceCallback : ILogInServiceCallback
    {
        public OperationResult LastOperationResult { get; private set; }
        public PlayerDTO LastPlayerInfo { get; private set; }

        public void LogInResult(OperationResult result)
        {
            LastOperationResult = result;
        }

        public void AccountInfo(PlayerDTO player)
        {
            LastPlayerInfo = player;
        }
    }

    [TestClass]
    public class LogInServiceClientTests
    {
        private TestLogInServiceCallback _callback;
        private LogInServiceClient _client;

        [TestInitialize]
        public void Setup()
        {
            _callback = new TestLogInServiceCallback();

            var instanceContext = new InstanceContext(_callback);
            _client = new LogInServiceClient(instanceContext);
        }

        [TestMethod]
        public async Task TestLogIn_Success()
        {
            await _client.LogInAsync("pikachu999@gmail.com", "407d8a1148a12157d5c1509f328461f6488a405bd4d361363b22be741fe1b1ee");

            Assert.AreEqual("Login successful", _callback.LastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestLogIn_InvalidCredentials()
        {
            await _client.LogInAsync("logInTest@gmail.com", "hashed_password");

            Assert.AreEqual("Invalid credentials", _callback.LastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestLogIn_InBlankCredentials()
        {
            await _client.LogInAsync("", "");

            Assert.AreEqual("Invalid credentials", _callback.LastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestLogIn_VerifyAccountInfo()
        {
            await _client.LogInAsync("pikachu999@gmail.com", "407d8a1148a12157d5c1509f328461f6488a405bd4d361363b22be741fe1b1ee");

            Assert.IsNotNull(_callback.LastPlayerInfo, "No se recibió información del jugador.");

            Assert.AreEqual("Pikachu999", _callback.LastPlayerInfo.Name);
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
