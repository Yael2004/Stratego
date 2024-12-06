using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using StrategoDataAccess;
using Utilities;
using System.Linq;
using System.Collections.Generic;

namespace Tests
{
    public class FakeDbSet<T> : DbSet<T>, IQueryable, IEnumerable<T> where T : class
    {
        private readonly List<T> _data;

        public FakeDbSet()
        {
            _data = new List<T>();
        }

        public override T Add(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public override T Remove(T entity)
        {
            _data.Remove(entity);
            return entity;
        }

        public override T Find(params object[] keyValues)
        {
            return _data.FirstOrDefault();
        }

        public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _data.GetEnumerator();

        Type IQueryable.ElementType => typeof(T);

        System.Linq.Expressions.Expression IQueryable.Expression => _data.AsQueryable().Expression;

        IQueryProvider IQueryable.Provider => _data.AsQueryable().Provider;
    }

    [TestClass]
    public class PictureRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private FakeDbSet<Pictures> _fakePictureSet;
        private PictureRepository _pictureRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<StrategoEntities>();

            _fakePictureSet = new FakeDbSet<Pictures>
            {
                new Pictures { IdPicture = 1, path = "http://example.com/image1.jpg" }
            };

            _mockContext.Setup(c => c.Pictures).Returns(_fakePictureSet);

            _pictureRepository = new PictureRepository();
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldReturnPicture_WhenPictureExists()
        {
            var pictureId = 1;

            var result = _pictureRepository.GetPictureById(pictureId);

            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldReturnFailure_WhenPictureDoesNotExist()
        {
            var pictureId = 2;

            var result = _pictureRepository.GetPictureById(pictureId);

            Assert.AreEqual("Picture not found", result.Error);
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldHandleSqlException()
        {
            var pictureId = 1;
            _mockContext.Setup(c => c.Pictures).Throws(new InvalidOperationException("Simulated database error"));

            var result = _pictureRepository.GetPictureById(pictureId);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void Test_GetPictureById_ShouldHandleUnexpectedException()
        {
            var pictureId = 1;
            _mockContext.Setup(c => c.Pictures).Throws(new Exception("Unexpected error"));

            var result = _pictureRepository.GetPictureById(pictureId);

            Assert.IsTrue(result.Error.Contains("Unexpected error"));
        }
    }
}
