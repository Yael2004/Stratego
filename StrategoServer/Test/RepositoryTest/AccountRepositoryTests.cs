using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using StrategoDataAccess;
using Utilities;
using Test.RepositoryTest;

namespace Tests
{
    [TestClass]
    public class AccountRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private FakeDbSet<Account> _fakeAccountSet;
        private AccountRepository _accountRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();

            _fakeAccountSet = new FakeDbSet<Account>
            {
                new Account { mail = "valid@example.com", password = "hashedPassword", IdAccount = 1 }
            };

            _mockContext.Setup(c => c.Account).Returns(_fakeAccountSet);

            _accountRepository = new AccountRepository(new Lazy<StrategoEntities>(() => _mockContext.Object));
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldReturnSuccess_WhenAccountIsCreated()
        {
            var email = "test@example.com";
            var hashedPassword = "hashedPassword";
            var playerName = "TestPlayer";

            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var result = _accountRepository.CreateAccount(email, hashedPassword, playerName);

            Assert.AreEqual("Account and player created successfully", result.Value);
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldReturnFailure_WhenAccountAlreadyExists()
        {
            var email = "valid@example.com";

            var result = _accountRepository.CreateAccount(email, "hashedPassword", "ExistingPlayer");

            Assert.AreEqual("Account already exists", result.Error);
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldHandleDbEntityValidationException()
        {
            var email = "test@example.com";
            _mockContext.Setup(m => m.SaveChanges()).Throws(new DbEntityValidationException("Validation error"));

            var result = _accountRepository.CreateAccount(email, "hashedPassword", "TestPlayer");

            Assert.IsTrue(result.Error.Contains("Entity validation error"));
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            var email = "valid@example.com";
            var hashedPassword = "hashedPassword";

            var result = _accountRepository.ValidateCredentials(email, hashedPassword);

            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldReturnFailure_WhenCredentialsAreInvalid()
        {
            var email = "invalid@example.com";
            var hashedPassword = "wrongPassword";

            var result = _accountRepository.ValidateCredentials(email, hashedPassword);

            Assert.AreEqual("Invalid credentials", result.Error);
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldHandleDbEntityValidationException()
        {
            var email = "valid@example.com";
            var hashedPassword = "hashedPassword";
            _mockContext.Setup(c => c.Account).Throws(new DbEntityValidationException("Validation error"));

            var result = _accountRepository.ValidateCredentials(email, hashedPassword);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Contains("Entity validation error"));
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldHandleUnexpectedException()
        {
            var email = "valid@example.com";
            var hashedPassword = "hashedPassword";
            _mockContext.Setup(c => c.Account).Throws(new Exception("Unexpected error"));

            var result = _accountRepository.ValidateCredentials(email, hashedPassword);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }

        [TestMethod]
        public void Test_AlreadyExistentAccount_ShouldReturnTrue_WhenAccountExists()
        {
            var email = "valid@example.com";

            var result = _accountRepository.AlreadyExistentAccount(email);

            Assert.IsTrue(result.Value);
        }

        [TestMethod]
        public void Test_AlreadyExistentAccount_ShouldReturnFalse_WhenAccountDoesNotExist()
        {
            var email = "new@example.com";

            var result = _accountRepository.AlreadyExistentAccount(email);

            Assert.IsFalse(result.Value);
        }

        [TestMethod]
        public void Test_AlreadyExistentAccount_ShouldHandleSqlException()
        {
            var email = "valid@example.com";
            _mockContext.Setup(c => c.Account).Throws(new InvalidOperationException("Simulated database error"));

            var result = _accountRepository.AlreadyExistentAccount(email);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void Test_ChangePassword_ShouldReturnSuccess_WhenPasswordIsChanged()
        {
            var email = "valid@example.com";
            var newHashedPassword = "newHashedPassword";
            var account = _fakeAccountSet.FirstOrDefault(a => a.mail == email);

            if (account != null)
            {
                account.password = newHashedPassword;
                _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            }

            var result = _accountRepository.ChangePassword(email, newHashedPassword);

            Assert.AreEqual("Password changed successfully", result.Value);
        }

        [TestMethod]
        public void Test_ChangePassword_ShouldReturnFailure_WhenAccountDoesNotExist()
        {
            var email = "nonexistent@example.com";

            var result = _accountRepository.ChangePassword(email, "newHashedPassword");

            Assert.AreEqual("Account does not exist", result.Error);
        }

        [TestMethod]
        public void Test_ChangePassword_ShouldHandleDbEntityValidationException()
        {
            var email = "valid@example.com";
            var newHashedPassword = "newHashedPassword";
            _mockContext.Setup(m => m.SaveChanges()).Throws(new DbEntityValidationException("Validation error"));

            var result = _accountRepository.ChangePassword(email, newHashedPassword);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Contains("Entity validation error"));
        }

        [TestMethod]
        public void Test_ChangePassword_ShouldHandleUnexpectedException()
        {
            var email = "valid@example.com";
            var newHashedPassword = "newHashedPassword";
            _mockContext.Setup(m => m.SaveChanges()).Throws(new Exception("Unexpected error"));

            var result = _accountRepository.ChangePassword(email, newHashedPassword);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }
    }
}
