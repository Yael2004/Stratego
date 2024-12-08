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
    [TestClass]
    public class LogInServiceClientTests : ILogInServiceCallback, IChangePasswordServiceCallback, ISignUpServiceCallback
    {
        private LogInServiceClient _client;
        private ChangePasswordServiceClient _changePasswordClient;
        private SignUpServiceClient _signUpClient;

        private string _lastPlayerName;
        private OperationResult _lastOperationResult;
        private OperationResult _changePasswordOperationResult;
        private OperationResult _signUpOperationResult;

        [TestInitialize]
        public void Setup()
        {
            _client = new LogInServiceClient(new InstanceContext(this));
            _changePasswordClient = new ChangePasswordServiceClient(new InstanceContext(this));
            _signUpClient = new SignUpServiceClient(new InstanceContext(this));
        }

        [TestMethod]
        public async Task TestLogIn_VerifyAccountInfo()
        {
            await _client.LogInAsync("armasgabriel29@gmail.com", "13df55fc30b4d971389218a808dc666538319bade3a99c10f6eb9aa8027c0552");
            
            Assert.AreEqual("Garmas2000", _lastPlayerName);
        }

        [TestMethod]
        public async Task TestLogIn_Success()
        {
            await _client.LogInAsync("armasgabo01@gmail.com", "407d8a1148a12157d5c1509f328461f6488a405bd4d361363b22be741fe1b1ee");

            Assert.AreEqual("Login successful", _lastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestLogIn_InvalidCredentials()
        {
            await _client.LogInAsync("logInTest@gmail.com", "hashed_password");

            Assert.AreEqual("Invalid credentials", _lastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestLogIn_InBlankCredentials()
        {
            await _client.LogInAsync("", "");

            Assert.AreEqual("Invalid credentials", _lastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestLogIn_InBlankEmail()
        {
            await _client.LogInAsync("", "hashed_password");

            Assert.AreEqual("Invalid credentials", _lastOperationResult.Message);
        }

        [TestMethod]
        public async Task TestLogIn_InBlankPassword()
        {
            await _client.LogInAsync("armasgabriel29@gmail.com", "");
            Assert.AreEqual("Invalid credentials", _lastOperationResult.Message);
        }

        [TestMethod]
        public async Task Test_ObtainVerificationCode_Success()
        {
            await _changePasswordClient.ObtainVerificationCodeAsync("armasgabriel29@gmail.com");
            Assert.AreEqual("Verification code sent.", _changePasswordOperationResult.Message);
        }

        [TestMethod]
        public async Task Test_ObtainVerificationCode_InBlankEmail()
        {
            await _changePasswordClient.ObtainVerificationCodeAsync("");
            Assert.AreEqual("Account not found", _changePasswordOperationResult.Message);
        }

        [TestMethod]
        public async Task Test_ObtainVerificationCode_UnexistentEmail()
        {
            await _changePasswordClient.ObtainVerificationCodeAsync("fakemail@gmail.com");
            Assert.AreEqual("Account not found", _changePasswordOperationResult.Message);
        }

        [TestMethod]
        public async Task Test_ValidateVerificacionCode_FakeMail_FakeCode()
        {
            await _changePasswordClient.SendVerificationCodeAsync("fakemail@gmail.com", "000000");
            Assert.AreEqual("Invalid verification code", _changePasswordOperationResult.Message);
        }

        [TestMethod]
        public async Task Test_ValidateVerificacionCode_RealMail_FakeCode()
        {
            await _changePasswordClient.SendVerificationCodeAsync("gaboarmas796@gmail.com", "000000");
            Assert.AreEqual("Invalid verification code", _changePasswordOperationResult.Message);
        }

        [TestMethod]
        public async Task Test_ChangePassword_Success()
        {
            await _changePasswordClient.SendNewPasswordAsync("gaboarmas796@gmail.com", "1f4775c7476dd2c8675584e816d14fe217d4d9ff1f2b18c211938eb5379634a6");
            Assert.AreEqual("Password changed successfully", _changePasswordOperationResult.Message);
        }

        [TestMethod]
        public async Task Test_CreateAccount_Success()
        {
            await _signUpClient.SignUpAsync("examplemail@gmail.com", "1f4775c7476dd2c8675584e816d14fe217d4d9ff1f2b18c211938eb5379634a6", "example2000");
            Assert.AreEqual("Account created successfully", _signUpOperationResult.Message);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_client.State == CommunicationState.Opened)
            {
                _client.Close();
            }
        }

        public void LogInResult(OperationResult result)
        {
            _lastOperationResult = result;
        }

        public void AccountInfo(PlayerDTO player)
        {
            _lastPlayerName = player.Name;
        }

        public void ChangePasswordResult(OperationResult result)
        {
            _changePasswordOperationResult = result;
        }

        public void SignUpResult(OperationResult result)
        {
            _signUpOperationResult = result;
        }
    }

}
