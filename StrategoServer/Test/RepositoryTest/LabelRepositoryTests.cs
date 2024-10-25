using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.RepositoryTest
{
    [TestClass]
    public class LabelRepositoryTests
    {
        private Mock<StrategoEntities> _mockContext;
        private Mock<DbSet<Label>> _mockLabelSet;
        private Lazy<StrategoEntities> _lazyMockContext;

        [TestInitialize]
        public void Initialize()
        {
            _mockContext = new Mock<StrategoEntities>();

            var labels = new List<Label>
            {
                new Label { IdLabel = 1, Path = "Label1" },
                new Label { IdLabel = 2, Path = "Label2" }
            };

            _mockLabelSet = CreateMockDbSet(labels);

            _mockContext.Setup(c => c.Label).Returns(_mockLabelSet.Object);

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
        public async Task GetLabelByIdAsync_ShouldReturnLabelWhenFound()
        {
            var repository = new LabelRepository(_lazyMockContext);
            var labelId = 1;
            var label = new Label { IdLabel = labelId, Path = "label1.jpg" };

            var mockLabelSet = CreateMockDbSet(new List<Label> { label });
            _mockContext.Setup(c => c.Label).Returns(mockLabelSet.Object);

            var result = await repository.GetLabelByIdAsync(labelId);

            Assert.AreEqual(label, result.Value);
        }

        [TestMethod]
        public async Task GetLabelByIdAsync_ShouldReturnFailureWhenNotFound()
        {
            var repository = new LabelRepository(_lazyMockContext);
            var labelId = 1;

            var mockLabelSet = CreateMockDbSet(new List<Label>()); 
            _mockContext.Setup(c => c.Label).Returns(mockLabelSet.Object);

            var result = await repository.GetLabelByIdAsync(labelId);

            Assert.AreEqual("Label not found", result.Error);
        }

        [TestMethod]
        public async Task GetLabelByIdAsync_ShouldReturnFailureOnSqlException()
        {
            var repository = new LabelRepository(_lazyMockContext);
            var labelId = 1;

            var mockLabelSet = new Mock<DbSet<Label>>();
            mockLabelSet.As<IQueryable<Label>>().Setup(m => m.Provider).Throws(new InvalidOperationException("Database error"));

            _mockContext.Setup(c => c.Label).Returns(mockLabelSet.Object);

            var result = await repository.GetLabelByIdAsync(labelId);

            Assert.IsTrue(result.Error.Contains("Database error"));
        }
    }

}
