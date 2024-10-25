using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using StrategoDataAccess;
using Utilities;
using System;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Test.RepositoryTest;

namespace Test
{
    [TestClass]
    public class AccountRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Mock<DbSet<Account>> _mockAccountSet;
        private Mock<DbSet<Player>> _mockPlayerSet;
        private Lazy<StrategoEntities> _lazyMockContext;

        [TestInitialize]
        public void Initialize()
        {
            _mockContext = new Mock<StrategoEntities>();

            _mockAccountSet = new Mock<DbSet<Account>>();
            _mockPlayerSet = new Mock<DbSet<Player>>();

            _mockContext.Setup(c => c.Account).Returns(_mockAccountSet.Object);
            _mockContext.Setup(c => c.Player).Returns(_mockPlayerSet.Object);

            _lazyMockContext = new Lazy<StrategoEntities>(() => _mockContext.Object);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mockSet.As<IDbAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockSet;
        }


        [TestMethod]
        public async Task CreateAccountAsync_ShouldReturnFailureIfAccountExists()
        {
            var repository = new AccountRepository(_lazyMockContext);
            string testEmail = "test@example.com";
            string testHashedPassword = "hashedPassword123";
            string testPlayerName = "TestPlayer";

            var existingAccounts = new List<Account>
            {
                new Account { mail = testEmail }
            };

            var mockAccountSet = CreateMockDbSet(existingAccounts);
            _mockContext.Setup(c => c.Account).Returns(mockAccountSet.Object);

            var result = await repository.CreateAccountAsync(testEmail, testHashedPassword, testPlayerName);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Account already exists", result.Error);

            _mockAccountSet.Verify(m => m.Add(It.IsAny<Account>()), Times.Never);
            _mockPlayerSet.Verify(m => m.Add(It.IsAny<Player>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task CreateAccountAsync_ShouldReturnSuccessWhenAccountIsCreated()
        {
            var repository = new AccountRepository(_lazyMockContext);
            string testEmail = "test@example.com";
            string testHashedPassword = "hashedPassword123";
            string testPlayerName = "TestPlayer";

            var existingAccounts = new List<Account>();
            var mockAccountSet = CreateMockDbSet(existingAccounts);

            mockAccountSet.Setup(m => m.Add(It.IsAny<Account>())).Callback<Account>(account => existingAccounts.Add(account));
            _mockContext.Setup(c => c.Account).Returns(mockAccountSet.Object);

            var players = new List<Player>();
            var mockPlayerSet = CreateMockDbSet(players);
            mockPlayerSet.Setup(m => m.Add(It.IsAny<Player>())).Callback<Player>(player => players.Add(player));
            _mockContext.Setup(c => c.Player).Returns(mockPlayerSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync()).ReturnsAsync(1);

            var result = await repository.CreateAccountAsync(testEmail, testHashedPassword, testPlayerName);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Account and player created successfully", result.Value);   
        }



        [TestMethod]
        public async Task CreateAccountAsync_ShouldReturnFailureOnEntityValidationError()
        {
            var repository = new AccountRepository(_lazyMockContext);

            var existingAccounts = new List<Account>();
            var mockAccountSet = CreateMockDbSet(existingAccounts);
            _mockContext.Setup(c => c.Account).Returns(mockAccountSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync())
                        .ThrowsAsync(new DbEntityValidationException("Validation Error"));

            var result = await repository.CreateAccountAsync("test@example.com", "hashed_password", "Player1");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Entity validation error: Validation Error", result.Error);
        }



        [TestMethod]
        public async Task ValidateCredentialsAsync_ShouldReturnSuccessWhenCredentialsAreValid()
        {
            var validEmail = "test@example.com";
            var validPassword = "hashed_password";
            var account = new Account { IdAccount = 1, mail = validEmail, password = validPassword };

            var existingAccounts = new List<Account> { account };
            var mockAccountSet = CreateMockDbSet(existingAccounts);

            _mockContext.Setup(c => c.Account).Returns(mockAccountSet.Object);

            var accountRepo = new AccountRepository(_lazyMockContext);

            var result = await accountRepo.ValidateCredentialsAsync(validEmail, validPassword);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }


        [TestMethod]
        public async Task ValidateCredentialsAsync_ShouldReturnFailureWhenCredentialsAreInvalid()
        {
            var invalidEmail = "test@example.com";
            var invalidPassword = "wrong_password";

            var emptyAccounts = new List<Account>();

            var mockAccountSet = CreateMockDbSet(emptyAccounts);
            _mockContext.Setup(c => c.Account).Returns(mockAccountSet.Object);

            var accountRepo = new AccountRepository(_lazyMockContext);

            var result = await accountRepo.ValidateCredentialsAsync(invalidEmail, invalidPassword);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid credentials", result.Error);
        }


        [TestMethod]
        public async Task AlreadyExistentAccountAsync_ShouldReturnTrueWhenAccountExists()
        {
            var repository = new AccountRepository(_lazyMockContext);
            string testEmail = "test@example.com";

            var existingAccounts = new List<Account>
            {
                new Account { mail = testEmail }
            };

            var mockAccountSet = CreateMockDbSet(existingAccounts);
            _mockContext.Setup(c => c.Account).Returns(mockAccountSet.Object);

            var result = await repository.AlreadyExistentAccountAsync(testEmail);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }

    }
}