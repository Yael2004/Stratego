using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.SqlClient;
using System.Linq;
using Utilities;

namespace StrategoDataAccess.Tests
{
    [TestClass]
    public class LabelRepositoryTests
    {
        private Mock<LabelRepository> _mockLabelRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockLabelRepository = new Mock<LabelRepository>();
        }

        [TestMethod]
        public void Test_GetLabelById_ShouldReturnLabel_WhenLabelExists()
        {
            var labelId = 1;
            var expectedLabel = new Label { IdLabel = labelId, Path = "Test Label" };

            _mockLabelRepository
                .Setup(repo => repo.GetLabelById(labelId))
                .Returns(Result<Label>.Success(expectedLabel));

            var result = _mockLabelRepository.Object.GetLabelById(labelId);

            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void Test_GetLabelById_ShouldReturnFailure_WhenLabelDoesNotExist()
        {
            var labelId = 999;

            _mockLabelRepository
                .Setup(repo => repo.GetLabelById(labelId))
                .Returns(Result<Label>.Failure("Label not found"));

            var result = _mockLabelRepository.Object.GetLabelById(labelId);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void Test_GetLabelById_ShouldReturnFailure_WhenUnexpectedErrorOccurs()
        {
            var labelId = 1;

            _mockLabelRepository
                .Setup(repo => repo.GetLabelById(labelId))
                .Throws(new Exception("Unexpected error"));

            var ex = Assert.ThrowsException<Exception>(() => _mockLabelRepository.Object.GetLabelById(labelId));
            Assert.AreEqual("Unexpected error", ex.Message);
        }
        public static class SqlExceptionHelper
        {
            public static SqlException Create(string message, int errorCode = 0)
            {
                var sqlErrorNumber = errorCode;
                var sqlError = typeof(SqlError).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0]
                    .Invoke(new object[] { sqlErrorNumber, (byte)0, (byte)0, "ServerName", message, "ProcedureName", 0 });

                var sqlErrorCollection = typeof(SqlErrorCollection)
                    .GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0]
                    .Invoke(null);

                var addMethod = typeof(SqlErrorCollection).GetMethod("Add", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                addMethod.Invoke(sqlErrorCollection, new[] { sqlError });

                var sqlException = typeof(SqlException).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0]
                    .Invoke(new object[] { message, sqlErrorCollection, null, Guid.NewGuid() });

                return (SqlException)sqlException;
            }
        }

        [TestMethod]
        public void Test_GetLabelById_ShouldThrowException_WhenDatabaseErrorOccurs()
        {
            var labelId = 1;

            _mockLabelRepository
                .Setup(repo => repo.GetLabelById(It.IsAny<int>()))
                .Throws(new Exception("Database error"));

            var ex = Assert.ThrowsException<Exception>(() => _mockLabelRepository.Object.GetLabelById(labelId));
            Assert.AreEqual("Database error", ex.Message);
        }

    }
}
