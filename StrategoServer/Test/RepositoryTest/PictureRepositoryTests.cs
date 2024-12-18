﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.SqlClient;
using Utilities;

namespace StrategoDataAccess.Tests
{
    [TestClass]
    public class PictureRepositoryTests
    {
        private Mock<PictureRepository> _mockPictureRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockPictureRepository = new Mock<PictureRepository>();
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldReturnPicture_WhenPictureExists()
        {
            var pictureId = 1;
            var expectedPicture = new Pictures { IdPicture = pictureId, path = "Test Picture" };

            _mockPictureRepository
                .Setup(repo => repo.GetPictureById(pictureId))
                .Returns(Result<Pictures>.Success(expectedPicture));

            var result = _mockPictureRepository.Object.GetPictureById(pictureId);

            Assert.AreEqual(expectedPicture, result.Value);
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldReturnFailure_WhenPictureDoesNotExist()
        {
            var pictureId = 999;

            _mockPictureRepository
                .Setup(repo => repo.GetPictureById(pictureId))
                .Returns(Result<Pictures>.Failure("Picture not found"));

            var result = _mockPictureRepository.Object.GetPictureById(pictureId);

            Assert.AreEqual("Picture not found", result.Error);
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldThrowException_WhenUnexpectedErrorOccurs()
        {
            var pictureId = 1;

            _mockPictureRepository
                .Setup(repo => repo.GetPictureById(pictureId))
                .Throws(new Exception("Unexpected error"));

            var ex = Assert.ThrowsException<Exception>(() => _mockPictureRepository.Object.GetPictureById(pictureId));
            Assert.AreEqual("Unexpected error", ex.Message);
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldThrowException_WhenDatabaseErrorOccurs()
        {
            var pictureId = 1;

            _mockPictureRepository
                .Setup(repo => repo.GetPictureById(It.IsAny<int>()))
                .Throws(new Exception("Database error"));

            var ex = Assert.ThrowsException<Exception>(() => _mockPictureRepository.Object.GetPictureById(pictureId));
            Assert.AreEqual("Database error", ex.Message);
        }
    }
}
