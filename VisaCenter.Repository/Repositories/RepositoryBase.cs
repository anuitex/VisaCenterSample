using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VisaCenter.Repository.Interfaces;

namespace VisaCenter.Repository.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly IServiceScopeFactory _scopeFactory;

        protected RepositoryBase(ApplicationDbContext dbContext, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected ApplicationDbContext DbContext { get; private set; }

        protected abstract DbSet<TEntity> DbSet { get; }

        public virtual IQueryable<TEntity> All => DbSet.AsNoTracking();

        public async Task<TEntity> CreateAsync(TEntity item)
        {
            var entity = DbSet.Add(item).Entity;
            await DbContext?.SaveChangesAsync();
            return entity;
        }

        public async Task<IList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await All.ToListAsync();
            }

            return All.Where(predicate.Compile()).ToList();
        }

        public async Task RemoveAsync(int id)
        {
            var entity = All.FirstOrDefault(x => x.Id == id);
            DbSet.Remove(entity);
            await DbContext?.SaveChangesAsync();
        }

        //public async Task UpdateAsync(TEntity item)
        //{
        //        if (DbContext.Entry(item).State == EntityState.Detached)
        //        {
        //            DbSet.Attach(item);
        //        }

        //        DbSet.Update(item);

        //        await DbContext?.SaveChangesAsync();
     
        //}

        #region solution
        public async Task UpdateAsync(TEntity item)
        {
            await ExecuteContextTask(async () =>
            {
                if (DbContext.Entry(item).State == EntityState.Detached)
                {
                    DbSet.Attach(item);
                }

                DbSet.Update(item);

                await DbContext?.SaveChangesAsync();
            });


        }


        private async Task ExecuteContextTask(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (ObjectDisposedException)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await action();
                }
            }

        }
        #endregion


    }
}
