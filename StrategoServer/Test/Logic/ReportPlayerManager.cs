using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using Utilities;
using StrategoServices.Logic;
using System;

namespace StrategoServices.Tests
{
    [TestClass]
    public class ReportPlayerManagerTests
    {
        private Mock<PlayerRepository> _playerRepositoryMock;
        private ReportPlayerManager _reportPlayerManager;

        [TestInitialize]
        public void Setup()
        {
            _playerRepositoryMock = new Mock<PlayerRepository>();
            _reportPlayerManager = new ReportPlayerManager(new Lazy<PlayerRepository>(() => _playerRepositoryMock.Object));
        }

        [TestMethod]
        public void ReportPlayer_ValidReport_ReturnsSuccess()
        {
            // Arrange
            int reporterId = 1;
            int reportedId = 2;
            string reason = "Cheating";

            var result = Result<string>.Success("Report submitted successfully");

            // Setup mock to return success result
            _playerRepositoryMock.Setup(repo => repo.ReportPlayer(reporterId, reportedId, reason))
                .Returns(result);

            // Act
            var actualResult = _reportPlayerManager.ReportPlayer(reporterId, reportedId, reason);

            // Assert
            Assert.IsTrue(actualResult.IsSuccess);
            Assert.AreEqual("Report submitted successfully", actualResult.Value);
        }

        [TestMethod]
        public void ReportPlayer_FailedReport_ReturnsFailure()
        {
            // Arrange
            int reporterId = 1;
            int reportedId = 2;
            string reason = "Cheating";

            var result = Result<string>.Failure("Failed to submit report");

            // Setup mock to return failure result
            _playerRepositoryMock.Setup(repo => repo.ReportPlayer(reporterId, reportedId, reason))
                .Returns(result);

            // Act
            var actualResult = _reportPlayerManager.ReportPlayer(reporterId, reportedId, reason);

            // Assert
            Assert.IsFalse(actualResult.IsSuccess);
            Assert.AreEqual("Failed to submit report", actualResult.Error);
        }
    }
}
