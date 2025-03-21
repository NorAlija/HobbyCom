using System.Linq.Expressions;

namespace HobbyCom.Domain.src.IRepositories
{
    public interface IBaseRepository<T>
    {
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
    }
}