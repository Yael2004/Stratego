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
            // Crear el Mock para el repositorio de cuentas
            _accountRepositoryMock = new Mock<AccountRepository>();

            // Inicializar el PasswordManager con el Mock
            _passwordManager = new PasswordManager(new Lazy<AccountRepository>(() => _accountRepositoryMock.Object));
        }

        [TestMethod]
        public void AlreadyExistentAccount_AccountExists_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            var expectedResult = Result<bool>.Success(true);

            // Configurar el comportamiento del Mock
            _accountRepositoryMock.Setup(repo => repo.AlreadyExistentAccount(email))
                .Returns(expectedResult);

            // Act
            var result = _passwordManager.AlreadyExistentAccount(email);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }

        [TestMethod]
        public void AlreadyExistentAccount_AccountDoesNotExist_ReturnsFalse()
        {
            // Arrange
            string email = "nonexistent@example.com";
            var expectedResult = Result<bool>.Success(false);

            // Configurar el comportamiento del Mock
            _accountRepositoryMock.Setup(repo => repo.AlreadyExistentAccount(email))
                .Returns(expectedResult);

            // Act
            var result = _passwordManager.AlreadyExistentAccount(email);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Value);
        }

        [TestMethod]
        public void ChangePassword_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            string email = "test@example.com";
            string newPassword = "hashedPassword123";
            var expectedResult = Result<string>.Success("Password changed successfully");

            // Configurar el comportamiento del Mock
            _accountRepositoryMock.Setup(repo => repo.ChangePassword(email, newPassword))
                .Returns(expectedResult);

            // Act
            var result = _passwordManager.ChangePassword(email, newPassword);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Password changed successfully", result.Value);
        }

        [TestMethod]
        public void ChangePassword_InvalidRequest_ReturnsFailure()
        {
            // Arrange
            string email = "test@example.com";
            string newPassword = "hashedPassword123";
            var expectedResult = Result<string>.Failure("Failed to change password");

            // Configurar el comportamiento del Mock
            _accountRepositoryMock.Setup(repo => repo.ChangePassword(email, newPassword))
                .Returns(expectedResult);

            // Act
            var result = _passwordManager.ChangePassword(email, newPassword);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to change password", result.Error);
        }

        [TestMethod]
        public void GenerateVerificationCode_ValidEmail_ReturnsCode()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            var code = _passwordManager.GenerateVerificationCode(email);

            // Assert
            Assert.IsNotNull(code);
            Assert.AreEqual(6, code.Length); // Código de 6 dígitos
        }

        [TestMethod]
        public void ValidateVerificationCode_ValidCode_ReturnsSuccess()
        {
            // Arrange
            string email = "test@example.com";
            string validCode = _passwordManager.GenerateVerificationCode(email);

            // Act
            var result = _passwordManager.ValidateVerificationCode(email, validCode);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }

        [TestMethod]
        public void ValidateVerificationCode_InvalidCode_ReturnsFailure()
        {
            // Arrange
            string email = "test@example.com";
            _passwordManager.GenerateVerificationCode(email); // Generar un código válido
            string invalidCode = "000000";

            // Act
            var result = _passwordManager.ValidateVerificationCode(email, invalidCode);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid verification code", result.Error);
        }

        [TestMethod]
        public void ValidateVerificationCode_CodeNotGenerated_ReturnsFailure()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";

            // Act
            var result = _passwordManager.ValidateVerificationCode(email, code);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid verification code", result.Error);
        }
    }
}
