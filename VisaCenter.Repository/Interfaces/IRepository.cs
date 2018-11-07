using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VisaCenter.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> All { get; }
        Task<TEntity> CreateAsync(TEntity item);
        Task<IList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate = null);
        Task RemoveAsync(int id);
        Task UpdateAsync(TEntity item);
    }
}
