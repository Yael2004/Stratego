using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using Utilities;
using System;

namespace Tests.Logic
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
        public void Setup()
        {
            _mockAccountRepository = new Mock<AccountRepository>(new Lazy<StrategoEntities>());
            _mockPlayerRepository = new Mock<PlayerRepository>(new Lazy<StrategoEntities>());
            _mockPictureRepository = new Mock<PictureRepository>(new Lazy<StrategoEntities>());
            _mockLabelRepository = new Mock<LabelRepository>(new Lazy<StrategoEntities>());

            _accountManager = new AccountManager(
                new Lazy<AccountRepository>(() => _mockAccountRepository.Object),
                new Lazy<PlayerRepository>(() => _mockPlayerRepository.Object),
                new Lazy<PictureRepository>(() => _mockPictureRepository.Object),
                new Lazy<LabelRepository>(() => _mockLabelRepository.Object));
        }

        [TestMethod]
        public void CreateAccount_ShouldReturnSuccess_WhenAccountIsCreated()
        {
            _mockAccountRepository.Setup(repo => repo.CreateAccount("test@example.com", "password", "TestPlayer"))
                .Returns(Result<string>.Success("Account created"));

            var result = _accountManager.CreateAccount("test@example.com", "password", "TestPlayer");

            Assert.AreEqual("Account created", result.Value);
        }

        [TestMethod]
        public void CreateAccount_ShouldReturnFailure_WhenAccountCreationFails()
        {
            _mockAccountRepository.Setup(repo => repo.CreateAccount("test@example.com", "password", "TestPlayer"))
                .Returns(Result<string>.Failure("Account creation failed"));

            var result = _accountManager.CreateAccount("test@example.com", "password", "TestPlayer");

            Assert.AreEqual("Account creation failed", result.Error);
        }

        [TestMethod]
        public void LogInAccount_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            _mockAccountRepository.Setup(repo => repo.ValidateCredentials("test@example.com", "password"))
                .Returns(Result<int>.Success(1));

            var result = _accountManager.LogInAccount("test@example.com", "password");

            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public void LogInAccount_ShouldReturnFailure_WhenCredentialsAreInvalid()
        {
            _mockAccountRepository.Setup(repo => repo.ValidateCredentials("test@example.com", "password"))
                .Returns(Result<int>.Failure("Invalid credentials"));

            var result = _accountManager.LogInAccount("test@example.com", "password");

            Assert.AreEqual("Invalid credentials", result.Error);
        }

        [TestMethod]
        public void GetLogInAccount_ShouldReturnSuccess_WhenDataIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 1, IdLabel = 1, AccountId = 1 };
            var picture = new Pictures { IdPicture = 1, path = "picturePath" };
            var label = new Label { IdLabel = 1, Path = "labelPath" };

            _mockPlayerRepository.Setup(repo => repo.GetPlayerByAccountId(1)).Returns(Result<Player>.Success(player));
            _mockPictureRepository.Setup(repo => repo.GetPictureById(1)).Returns(Result<Pictures>.Success(picture));
            _mockLabelRepository.Setup(repo => repo.GetLabelById(1)).Returns(Result<Label>.Success(label));

            var result = _accountManager.GetLogInAccount(1);

            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void GetLogInAccount_ShouldReturnCorrectPlayerName_WhenDataIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 1, IdLabel = 1, AccountId = 1 };
            var picture = new Pictures { IdPicture = 1, path = "picturePath" };
            var label = new Label { IdLabel = 1, Path = "labelPath" };

            _mockPlayerRepository.Setup(repo => repo.GetPlayerByAccountId(1)).Returns(Result<Player>.Success(player));
            _mockPictureRepository.Setup(repo => repo.GetPictureById(1)).Returns(Result<Pictures>.Success(picture));
            _mockLabelRepository.Setup(repo => repo.GetLabelById(1)).Returns(Result<Label>.Success(label));

            var result = _accountManager.GetLogInAccount(1);

            Assert.AreEqual("Player1", result.Value.Name);
        }

        [TestMethod]
        public void GetLogInAccount_ShouldReturnCorrectPicturePath_WhenDataIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 1, IdLabel = 1, AccountId = 1 };
            var picture = new Pictures { IdPicture = 1, path = "picturePath" };
            var label = new Label { IdLabel = 1, Path = "labelPath" };

            _mockPlayerRepository.Setup(repo => repo.GetPlayerByAccountId(1)).Returns(Result<Player>.Success(player));
            _mockPictureRepository.Setup(repo => repo.GetPictureById(1)).Returns(Result<Pictures>.Success(picture));
            _mockLabelRepository.Setup(repo => repo.GetLabelById(1)).Returns(Result<Label>.Success(label));

            var result = _accountManager.GetLogInAccount(1);

            Assert.AreEqual("picturePath", result.Value.PicturePath);
        }

        [TestMethod]
        public void GetLogInAccount_ShouldReturnCorrectLabelPath_WhenDataIsComplete()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 1, IdLabel = 1, AccountId = 1 };
            var picture = new Pictures { IdPicture = 1, path = "picturePath" };
            var label = new Label { IdLabel = 1, Path = "labelPath" };

            _mockPlayerRepository.Setup(repo => repo.GetPlayerByAccountId(1)).Returns(Result<Player>.Success(player));
            _mockPictureRepository.Setup(repo => repo.GetPictureById(1)).Returns(Result<Pictures>.Success(picture));
            _mockLabelRepository.Setup(repo => repo.GetLabelById(1)).Returns(Result<Label>.Success(label));

            var result = _accountManager.GetLogInAccount(1);

            Assert.AreEqual("labelPath", result.Value.LabelPath);
        }

        [TestMethod]
        public void GetLogInAccount_ShouldReturnFailure_WhenPlayerNotFound()
        {
            _mockPlayerRepository.Setup(repo => repo.GetPlayerByAccountId(1))
                .Returns(Result<Player>.Failure("Player not found"));

            var result = _accountManager.GetLogInAccount(1);

            Assert.AreEqual("Player not found", result.Error);
        }

        [TestMethod]
        public void GetLogInAccount_ShouldReturnFailure_WhenPictureNotFound()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 1, IdLabel = 1, AccountId = 1 };

            _mockPlayerRepository.Setup(repo => repo.GetPlayerByAccountId(1)).Returns(Result<Player>.Success(player));
            _mockPictureRepository.Setup(repo => repo.GetPictureById(1)).Returns(Result<Pictures>.Failure("Picture not found"));

            var result = _accountManager.GetLogInAccount(1);

            Assert.AreEqual("Picture not found", result.Error);
        }

        [TestMethod]
        public void GetLogInAccount_ShouldReturnFailure_WhenLabelNotFound()
        {
            var player = new Player { Id = 1, Name = "Player1", PictureId = 1, IdLabel = 1, AccountId = 1 };
            var picture = new Pictures { IdPicture = 1, path = "picturePath" };

            _mockPlayerRepository.Setup(repo => repo.GetPlayerByAccountId(1)).Returns(Result<Player>.Success(player));
            _mockPictureRepository.Setup(repo => repo.GetPictureById(1)).Returns(Result<Pictures>.Success(picture));
            _mockLabelRepository.Setup(repo => repo.GetLabelById(1)).Returns(Result<Label>.Failure("Label not found"));

            var result = _accountManager.GetLogInAccount(1);

            Assert.AreEqual("Label not found", result.Error);
        }
    }
}
