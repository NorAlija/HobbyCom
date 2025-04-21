using System.Linq.Expressions;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using Microsoft.EntityFrameworkCore;

namespace HobbyCom.Infrastructure.src.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected SupabaseContext _dbContext;
        protected readonly DbSet<T> _setDB;

        public BaseRepository(SupabaseContext dbcontext)
        {
            _dbContext = dbcontext;
            _setDB = _dbContext.Set<T>();
        }

        public virtual async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _setDB.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var result = await _setDB.AnyAsync(predicate);
            return result;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _setDB.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _setDB.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _setDB.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            _setDB.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}