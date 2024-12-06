using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoServices.Logic;
using StrategoDataAccess;
using Utilities;
using System.Collections.Generic;
using System;

namespace StrategoServices.Tests
{
    [TestClass]
    public class PasswordManagerTests
    {
        private PasswordManager _passwordManager;
        private Mock<AccountRepository> _accountRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            _accountRepositoryMock = new Mock<AccountRepository>();

            _passwordManager = new PasswordManager(new Lazy<AccountRepository>(() => _accountRepositoryMock.Object));
        }

        [TestMethod]
        public void Test_AlreadyExistentAccount_AccountExists_ReturnsTrue()
        {
            string email = "test@example.com";
            var expectedResult = Result<bool>.Success(true);

            _accountRepositoryMock.Setup(repo => repo.AlreadyExistentAccount(email))
                .Returns(expectedResult);

            var result = _passwordManager.AlreadyExistentAccount(email);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }

        [TestMethod]
        public void Test_AlreadyExistentAccount_AccountDoesNotExist_ReturnsFalse()
        {
            string email = "nonexistent@example.com";
            var expectedResult = Result<bool>.Success(false);

            _accountRepositoryMock.Setup(repo => repo.AlreadyExistentAccount(email))
                .Returns(expectedResult);

            var result = _passwordManager.AlreadyExistentAccount(email);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Value);
        }

        [TestMethod]
        public void Test_ChangePassword_ValidRequest_ReturnsSuccess()
        {
            string email = "test@example.com";
            string newPassword = "hashedPassword123";
            var expectedResult = Result<string>.Success("Password changed successfully");

            _accountRepositoryMock.Setup(repo => repo.ChangePassword(email, newPassword))
                .Returns(expectedResult);

            var result = _passwordManager.ChangePassword(email, newPassword);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Password changed successfully", result.Value);
        }

        [TestMethod]
        public void Test_ChangePassword_InvalidRequest_ReturnsFailure()
        {
            string email = "test@example.com";
            string newPassword = "hashedPassword123";
            var expectedResult = Result<string>.Failure("Failed to change password");

            _accountRepositoryMock.Setup(repo => repo.ChangePassword(email, newPassword))
                .Returns(expectedResult);

            var result = _passwordManager.ChangePassword(email, newPassword);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to change password", result.Error);
        }

        [TestMethod]
        public void Test_GenerateVerificationCode_ValidEmail_ReturnsCode()
        {
            string email = "test@example.com";

            var code = _passwordManager.GenerateVerificationCode(email);

            Assert.IsNotNull(code);
            Assert.AreEqual(6, code.Length);
        }

        [TestMethod]
        public void Test_ValidateVerificationCode_ValidCode_ReturnsSuccess()
        {
            string email = "test@example.com";
            string validCode = _passwordManager.GenerateVerificationCode(email);

            var result = _passwordManager.ValidateVerificationCode(email, validCode);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }

        [TestMethod]
        public void Test_ValidateVerificationCode_InvalidCode_ReturnsFailure()
        {
            string email = "test@example.com";
            _passwordManager.GenerateVerificationCode(email);
            string invalidCode = "000000";

            var result = _passwordManager.ValidateVerificationCode(email, invalidCode);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid verification code", result.Error);
        }

        [TestMethod]
        public void Test_ValidateVerificationCode_CodeNotGenerated_ReturnsFailure()
        {
            string email = "test@example.com";
            string code = "123456";

            var result = _passwordManager.ValidateVerificationCode(email, code);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid verification code", result.Error);
        }
    }
}
