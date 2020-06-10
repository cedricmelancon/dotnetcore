using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace UserApplication.Data
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        DbContext GetContext();
        int SaveChanges();
        Task<int> SaveChangesAsync();

        int ExecuteSqlCommand(string sql, params object[] parameters);
        IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
    }
}
