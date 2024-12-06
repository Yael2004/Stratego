using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace StrategoDataAccess.Tests
{
    [TestClass]
    public class AccountRepositoryTests
    {
        private Mock<IAccountRepository> _mockRepo;
        private AccountService _accountService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _accountService = new AccountService(_mockRepo.Object);
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldReturnSuccess_WhenAccountIsCreated()
        {
            var email = "newuser@example.com";
            var password = "hashedPassword";

            _mockRepo.Setup(repo => repo.CreateAccount(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _accountService.CreateAccount(email, password);

            Assert.IsTrue(result, "The account creation should return success.");
            _mockRepo.Verify(repo => repo.CreateAccount(email, password), Times.Once);
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldReturnFailure_WhenAccountAlreadyExists()
        {
            var email = "existinguser@example.com";
            var password = "hashedPassword";

            _mockRepo.Setup(repo => repo.CreateAccount(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _accountService.CreateAccount(email, password);

            Assert.IsFalse(result, "The account creation should fail if the account already exists.");
            _mockRepo.Verify(repo => repo.CreateAccount(email, password), Times.Once);
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldReturnSuccess_WhenValidCredentials()
        {
            var email = "valid@example.com";
            var password = "hashedPassword";

            _mockRepo.Setup(repo => repo.ValidateCredentials(email, password)).Returns(1);

            var result = _accountService.ValidateCredentials(email, password);

            Assert.AreEqual(1, result, "Valid credentials should return account ID 1.");
            _mockRepo.Verify(repo => repo.ValidateCredentials(email, password), Times.Once);
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldReturnFailure_WhenInvalidCredentials()
        {
            var email = "invalid@example.com";
            var password = "wrongPassword";

            _mockRepo.Setup(repo => repo.ValidateCredentials(email, password)).Returns(0);

            var result = _accountService.ValidateCredentials(email, password);

            Assert.AreEqual(0, result, "Invalid credentials should return account ID 0.");
            _mockRepo.Verify(repo => repo.ValidateCredentials(email, password), Times.Once);
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldThrowException_WhenDatabaseErrorOccurs()
        {
            var email = "erroruser@example.com";
            var password = "hashedPassword";

            _mockRepo.Setup(repo => repo.CreateAccount(It.IsAny<string>(), It.IsAny<string>()))
                     .Throws(new Exception("Database error"));

            var ex = Assert.ThrowsException<Exception>(() => _accountService.CreateAccount(email, password));
            Assert.AreEqual("Database error: Database error", ex.Message, "The exception message should match the expected format.");
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldThrowException_WhenDatabaseErrorOccurs()
        {
            var email = "error@example.com";
            var password = "hashedPassword";

            _mockRepo.Setup(repo => repo.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>()))
                     .Throws(new Exception("Database error"));

            var ex = Assert.ThrowsException<Exception>(() => _accountService.ValidateCredentials(email, password));
            Assert.AreEqual("Database error: Database error", ex.Message, "The exception message should match the expected format.");
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldReturnFailure_WhenEmailIsInvalid()
        {
            var email = "invalid-email";
            var password = "hashedPassword";

            var result = _accountService.CreateAccount(email, password);

            Assert.IsFalse(result, "The account creation should fail when the email is invalid.");
        }

        [TestMethod]
        public void Test_CreateAccount_ShouldReturnFailure_WhenPasswordIsWeak()
        {
            var email = "newuser@example.com";
            var password = "short";

            var result = _accountService.CreateAccount(email, password);

            Assert.IsFalse(result, "The account creation should fail when the password is too weak.");
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldReturnFailure_WhenEmailNotFound()
        {
            var email = "nonexistent@example.com";
            var password = "hashedPassword";

            _mockRepo.Setup(repo => repo.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(0); 

            var result = _accountService.ValidateCredentials(email, password);

            Assert.AreEqual(0, result, "The validation should fail when the email is not found.");
        }

        [TestMethod]
        public void Test_ValidateCredentials_ShouldReturnFailure_WhenPasswordIncorrect()
        {
            var email = "valid@example.com";
            var password = "wrongPassword"; 

            _mockRepo.Setup(repo => repo.ValidateCredentials(email, password))
                     .Returns(0);

            var result = _accountService.ValidateCredentials(email, password);

            Assert.AreEqual(0, result, "The validation should fail when the password is incorrect.");
        }
    }

    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public bool CreateAccount(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                    return false;

                if (password.Length < 6)
                    return false;

                return _accountRepository.CreateAccount(email, password);
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public int ValidateCredentials(string email, string password)
        {
            try
            {
                return _accountRepository.ValidateCredentials(email, password);
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
    }

    public interface IAccountRepository
    {
        bool CreateAccount(string email, string password);
        int ValidateCredentials(string email, string password);
    }
}
