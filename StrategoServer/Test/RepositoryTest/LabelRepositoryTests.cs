using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using StrategoDataAccess;
using Utilities;
using System.Linq;
using System.Collections.Generic;
using Test.RepositoryTest;

namespace Tests
{
    [TestClass]
    public class LabelRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private FakeDbSet<Label> _fakeLabelSet;
        private LabelRepository _labelRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();

            _fakeLabelSet = new FakeDbSet<Label>
            {
                new Label { IdLabel = 1, Path = "TestLabel" }
            };

            _mockContext.Setup(c => c.Label).Returns(_fakeLabelSet);

            _labelRepository = new LabelRepository(new Lazy<StrategoEntities>(() => _mockContext.Object));
        }

        [TestMethod]
        public void GetLabelById_ShouldReturnLabel_WhenLabelExists()
        {
            var labelId = 1;

            var result = _labelRepository.GetLabelById(labelId);

            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public void GetLabelById_ShouldReturnFailure_WhenLabelDoesNotExist()
        {
            var labelId = 2;

            var result = _labelRepository.GetLabelById(labelId);

            Assert.AreEqual("Label not found", result.Error);
        }

        [TestMethod]
        public void GetLabelById_ShouldHandleSqlException()
        {
            var labelId = 1;
            _mockContext.Setup(c => c.Label).Throws(new InvalidOperationException("Simulated database error"));

            var result = _labelRepository.GetLabelById(labelId);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void GetLabelById_ShouldHandleUnexpectedException()
        {
            var labelId = 1;
            _mockContext.Setup(c => c.Label).Throws(new Exception("Unexpected error"));

            var result = _labelRepository.GetLabelById(labelId);

            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }
    }
}
