using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using StrategoServices.Logic;
using Utilities;
using System;

namespace Tests.Logic
{
    [TestClass]
    public class PasswordManagerTests
    {
        private Mock<AccountRepository> _mockAccountRepository;
        private PasswordManager _passwordManager;

        [TestInitialize]
        public void Setup()
        {
            _mockAccountRepository = new Mock<AccountRepository>(new Lazy<StrategoEntities>());
            _passwordManager = new PasswordManager(new Lazy<AccountRepository>(() => _mockAccountRepository.Object));
        }

        [TestMethod]
        public void Test_AlreadyExistentAccount_ShouldReturnSuccess_WhenAccountExists()
        {
            _mockAccountRepository.Setup(repo => repo.AlreadyExistentAccount("test@example.com"))
                .Returns(Result<bool>.Success(true));

            var result = _passwordManager.AlreadyExistentAccount("test@example.com");

            Assert.IsTrue(result.Value);
        }

        [TestMethod]
        public void Test_AlreadyExistentAccount_ShouldReturnFailure_WhenAccountDoesNotExist()
        {
            _mockAccountRepository.Setup(repo => repo.AlreadyExistentAccount("test@example.com"))
                .Returns(Result<bool>.Success(false));

            var result = _passwordManager.AlreadyExistentAccount("test@example.com");

            Assert.IsFalse(result.Value);
        }

        [TestMethod]
        public void Test_ChangePassword_ShouldReturnSuccess_WhenPasswordIsChanged()
        {
            _mockAccountRepository.Setup(repo => repo.ChangePassword("test@example.com", "newHashedPassword"))
                .Returns(Result<string>.Success("Password changed successfully"));

            var result = _passwordManager.ChangePassword("test@example.com", "newHashedPassword");

            Assert.AreEqual("Password changed successfully", result.Value);
        }

        [TestMethod]
        public void Test_ChangePassword_ShouldReturnFailure_WhenChangeFails()
        {
            _mockAccountRepository.Setup(repo => repo.ChangePassword("test@example.com", "newHashedPassword"))
                .Returns(Result<string>.Failure("Change failed"));

            var result = _passwordManager.ChangePassword("test@example.com", "newHashedPassword");

            Assert.AreEqual("Change failed", result.Error);
        }

        [TestMethod]
        public void Test_GenerateVerificationCode_ShouldStoreCodeForEmail()
        {
            var email = "test@example.com";
            var code = _passwordManager.GenerateVerificationCode(email);

            Assert.AreEqual(6, code.Length);
        }

        [TestMethod]
        public void Test_ValidateVerificationCode_ShouldReturnSuccess_WhenCodeIsCorrect()
        {
            var email = "test@example.com";
            var code = _passwordManager.GenerateVerificationCode(email);

            var result = _passwordManager.ValidateVerificationCode(email, code);

            Assert.IsTrue(result.Value);
        }

        [TestMethod]
        public void Test_ValidateVerificationCode_ShouldReturnFailure_WhenCodeIsIncorrect()
        {
            var email = "test@example.com";
            _passwordManager.GenerateVerificationCode(email);

            var result = _passwordManager.ValidateVerificationCode(email, "wrongCode");

            Assert.AreEqual("Invalid verification code", result.Error);
        }

        [TestMethod]
        public void Test_ValidateVerificationCode_ShouldReturnFailure_WhenCodeDoesNotExist()
        {
            var result = _passwordManager.ValidateVerificationCode("test@example.com", "123456");

            Assert.AreEqual("Invalid verification code", result.Error);
        }
    }
}
