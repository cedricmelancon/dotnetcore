using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace UserApplication.Data
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        int Count(Expression<Func<T, bool>> predicate = null);

        T GetById(object id);

        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            bool asNoTracking = true);

        Task InsertAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        void Update(T entity);

        void Delete(T entity);

        void DeleteById(object id);
    }
}
