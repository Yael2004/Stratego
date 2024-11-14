using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.RepositoryTest
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
}
