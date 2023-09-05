using System.Linq.Expressions;
using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.Common;
using CareerHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Infrastructure.DataAccess
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DatabaseContext dbContext)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<TEntity?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> where)
        {
            return await _dbSet.AsNoTracking().MaxAsync(where);
        }

        public async Task<TEntity?> GetWhereAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] include)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (include != null)
            {
                query = include.Aggregate(query, (current, expression) => current.Include(expression));
            }

            return await query.FirstOrDefaultAsync(where);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }
    }
}
