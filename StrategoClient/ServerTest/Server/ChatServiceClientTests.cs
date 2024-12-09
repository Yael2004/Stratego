using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest.Server
{
    [TestClass]
    public class ChatServiceClientTests : IChatServiceCallback
    {
        private ChatServiceClient _client;
        private OperationResult _lastOperationResult;
        private string _lastMessage;

        [TestInitialize]
        public void Setup()
        {
            _client = new ChatServiceClient(new InstanceContext(this));
        }

        [TestMethod]
        public void Test_ConnectUser_Success()
        {
            var response = _client.Connect(2, "Garmas2000");
            Assert.AreNotEqual(0, response);
        }

        [TestMethod]
        public async Task Test_SendMessage_Success()
        {
            _client.Connect(2, "Garmas2000");
            _client.SendMessage(2, "Garmas2000", "Hello, world!");
            await Task.Delay(3000);
            Assert.AreEqual("Hello, world!", _lastMessage);
        }

        [TestMethod]
        public async Task Test_SendEmptyMessage_Success()
        {
            _client.Connect(2, "Garmas2000");
            _client.SendMessage(2, "Garmas2000", "");
            await Task.Delay(3000);
            Assert.AreEqual("", _lastMessage);
        }

        [TestMethod]
        public async Task Test_SendMessageFakeUserNameRealId_Success()
        {
            _client.Connect(2, "Garmas2000");
            _client.SendMessage(2, "XXXXX", "Hello, world!");
            await Task.Delay(3000);
            Assert.AreEqual("Hello, world!", _lastMessage);
        }

        [TestMethod]
        public async Task Test_SendMessageRealUserNameFakeId_Success()
        {
            _client.Connect(2, "Garmas2000");
            _client.SendMessage(55, "Garmas2000", "Hello, world!");
            await Task.Delay(3000);
            Assert.AreNotEqual("Hello, world!", _lastMessage);
        }

        [TestMethod]
        public async Task Test_DisconnectUser_Success()
        {
            _client.Disconnect(2);
            await Task.Delay(3000);
            Assert.AreEqual("User is not connected.", _lastOperationResult.Message);
        }

        public void ChatResponse(OperationResult result)
        {
            _lastOperationResult = result;
        }

        public void ReceiveMessage(string username, string message)
        {
            _lastMessage = message;
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
