using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Test.RepositoryTest
{
    [TestClass]
    public class PictureRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Mock<DbSet<Pictures>> _mockPictureSet;

        [TestInitialize]
        public void Initialize()
        {
            _mockContext = new Mock<StrategoEntities>();
            _mockPictureSet = new Mock<DbSet<Pictures>>();

            _mockContext.Setup(c => c.Pictures).Returns(_mockPictureSet.Object);
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
        public async Task GetPictureByIdAsync_ShouldReturnPictureWhenFound()
        {
            var repository = new PictureRepository(_mockContext.Object);
            var pictureId = 1;
            var picture = new Pictures { IdPicture = pictureId, path = "picture1.jpg" };

            var mockPictureSet = CreateMockDbSet(new List<Pictures> { picture });
            _mockContext.Setup(c => c.Pictures).Returns(mockPictureSet.Object);

            var result = await repository.GetPictureByIdAsync(pictureId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(picture, result.Value);
        }

        [TestMethod]
        public async Task GetPictureByIdAsync_ShouldReturnFailureWhenNotFound()
        {
            var repository = new PictureRepository(_mockContext.Object);
            var pictureId = 1;

            var mockPictureSet = CreateMockDbSet(new List<Pictures>());
            _mockContext.Setup(c => c.Pictures).Returns(mockPictureSet.Object);

            var result = await repository.GetPictureByIdAsync(pictureId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Picture not found", result.Error);
        }

        [TestMethod]
        public async Task GetPictureByIdAsync_ShouldReturnFailureOnSqlException()
        {
            var repository = new PictureRepository(_mockContext.Object);
            var pictureId = 1;

            var existingPictures = new List<Pictures>
            {
                new Pictures { IdPicture = pictureId, path = "picture1.jpg" }
            };

            var mockPictureSet = CreateMockDbSet(existingPictures);

            mockPictureSet.As<IQueryable<Pictures>>().Setup(m => m.Provider).Throws(new InvalidOperationException("Database error"));

            _mockContext.Setup(c => c.Pictures).Returns(mockPictureSet.Object);

            var result = await repository.GetPictureByIdAsync(pictureId);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Contains("Database error"));
        }


    }

}
