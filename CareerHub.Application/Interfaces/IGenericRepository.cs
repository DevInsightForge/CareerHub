using System.Linq.Expressions;

namespace CareerHub.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<T?> GetByIdAsync(long id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> where);
        Task<T?> GetWhereAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] include);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    }
}
