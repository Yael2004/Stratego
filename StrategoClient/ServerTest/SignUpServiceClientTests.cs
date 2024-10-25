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
    public class TestSignUpServiceCallback : ISignUpServiceCallback
    {
        public OperationResult LastOperationResult { get; private set; }

        public void SignUpResult(OperationResult result)
        {
            LastOperationResult = result;
        }
    }

    [TestClass]
    public class SignUpServiceClientTests
    {
        private TestSignUpServiceCallback _callback;
        private SignUpServiceClient _client;

        [TestInitialize]
        public void Setup()
        {
            _callback = new TestSignUpServiceCallback();

            var instanceContext = new InstanceContext(_callback);
            _client = new SignUpServiceClient(instanceContext);
        }

        [TestMethod]
        public async Task TestSignUp_Success()
        {
            await _client.SignUpAsync("SignUpTest@gmail.com", "StrongPassword123.", "TestPlayer");

            Assert.AreEqual("Account created successfully", _callback.LastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestSignUp_EmailAlreadyExists()
        {
            await _client.SignUpAsync("pikachu999@gmail.com", "407d8a1148a12157d5c1509f328461f6488a405bd4d361363b22be741fe1b1ee", "Pikachu999");

            Assert.AreEqual("Account already exists", _callback.LastOperationResult.Message);
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
