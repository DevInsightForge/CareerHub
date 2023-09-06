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
        private readonly IUnitOfWork _unitOfWork;

        public GenericRepository(DatabaseContext dbContext, IUnitOfWork unitOfWork)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }
            _dbSet = dbContext.Set<TEntity>();
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _dbSet.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await _dbSet.AddRangeAsync(entities);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
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
