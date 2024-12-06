using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoServices.Logic;
using StrategoDataAccess;
using StrategoServices.Data.DTO;
using Utilities;
using System;

namespace StrategoServices.Tests
{
    [TestClass]
    public class AccountManagerTests
    {
        private Mock<AccountRepository> _mockAccountRepository;
        private Mock<PlayerRepository> _mockPlayerRepository;
        private Mock<PictureRepository> _mockPictureRepository;
        private Mock<LabelRepository> _mockLabelRepository;
        private AccountManager _accountManager;

        [TestInitialize]
        public void SetUp()
        {
            _mockAccountRepository = new Mock<AccountRepository>();
            _mockPlayerRepository = new Mock<PlayerRepository>();
            _mockPictureRepository = new Mock<PictureRepository>();
            _mockLabelRepository = new Mock<LabelRepository>();

            _accountManager = new AccountManager(
                new Lazy<AccountRepository>(() => _mockAccountRepository.Object),
                new Lazy<PlayerRepository>(() => _mockPlayerRepository.Object),
                new Lazy<PictureRepository>(() => _mockPictureRepository.Object),
                new Lazy<LabelRepository>(() => _mockLabelRepository.Object)
            );
        }

        [TestMethod]
        public void Test_CreateAccount_ValidData_ReturnsSuccess()
        {
            var email = "test@example.com";
            var password = "password123";
            var playerName = "TestPlayer";

            _mockAccountRepository.Setup(x => x.CreateAccount(email, password, playerName))
                .Returns(Result<string>.Success("Account created successfully"));

            var result = _accountManager.CreateAccount(email, password, playerName);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Account created successfully", result.Value);
        }

        [TestMethod]
        public void Test_LogInAccount_ValidCredentials_ReturnsAccountId()
        {
            var email = "test@example.com";
            var password = "password123";
            var accountId = 1;
            var playerId = 2;
            var reportCount = 1;

            _mockAccountRepository.Setup(x => x.ValidateCredentials(email, password))
                .Returns(Result<int>.Success(accountId));

            _mockPlayerRepository.Setup(x => x.GetPlayerByAccountId(accountId))
                .Returns(Result<Player>.Success(new Player { Id = playerId }));

            _mockPlayerRepository.Setup(x => x.GetReportCount(playerId))
                .Returns(Result<int>.Success(reportCount));

            var result = _accountManager.LogInAccount(email, password);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(accountId, result.Value);
        }

        [TestMethod]
        public void Test_LogInAccount_TooManyReports_ReturnsFailure()
        {
            var email = "test@example.com";
            var password = "password123";
            var accountId = 1;
            var playerId = 2;
            var reportCount = 3;

            _mockAccountRepository.Setup(x => x.ValidateCredentials(email, password))
                .Returns(Result<int>.Success(accountId));

            _mockPlayerRepository.Setup(x => x.GetPlayerByAccountId(accountId))
                .Returns(Result<Player>.Success(new Player { Id = playerId }));

            _mockPlayerRepository.Setup(x => x.GetReportCount(playerId))
                .Returns(Result<int>.Success(reportCount));

            var result = _accountManager.LogInAccount(email, password);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Access denied: This account has been reported too many times.", result.Error);
        }

        [TestMethod]
        public void Test_GetLogInAccount_ValidAccount_ReturnsPlayerDTO()
        {
            var accountId = 1;
            var player = new Player { Id = 2, Name = "TestPlayer", AccountId = accountId, PictureId = 1, IdLabel = 1 };
            var picture = new Pictures { path = "picturePath" };
            var label = new Label { Path = "labelPath" };

            _mockPlayerRepository.Setup(x => x.GetPlayerByAccountId(accountId))
                .Returns(Result<Player>.Success(player));

            _mockPictureRepository.Setup(x => x.GetPictureById(player.PictureId))
                .Returns(Result<Pictures>.Success(picture));

            _mockLabelRepository.Setup(x => x.GetLabelById(player.IdLabel))
                .Returns(Result<Label>.Success(label));

            var result = _accountManager.GetLogInAccount(accountId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(player.Id, result.Value.Id);
            Assert.AreEqual("picturePath", result.Value.PicturePath);
            Assert.AreEqual("labelPath", result.Value.LabelPath);
        }

        [TestMethod]
        public void Test_GetLogInAccount_PlayerNotFound_ReturnsFailure()
        {
            var accountId = 1;

            _mockPlayerRepository.Setup(x => x.GetPlayerByAccountId(accountId))
                .Returns(Result<Player>.Failure("Player not found"));

            var result = _accountManager.GetLogInAccount(accountId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void Test_GetLogInAccount_PictureNotFound_ReturnsFailure()
        {
            var accountId = 1;
            var player = new Player { Id = 2, Name = "TestPlayer", AccountId = accountId, PictureId = 1, IdLabel = 1 };

            _mockPlayerRepository.Setup(x => x.GetPlayerByAccountId(accountId))
                .Returns(Result<Player>.Success(player));

            _mockPictureRepository.Setup(x => x.GetPictureById(player.PictureId))
                .Returns(Result<Pictures>.Failure("Picture not found"));

            var result = _accountManager.GetLogInAccount(accountId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Picture not found", result.Error);
        }
    }
}
