using System.Linq.Expressions;
using HobbyCom.Domain.src.Entities;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using Microsoft.EntityFrameworkCore;

namespace HobbyCom.Infrastructure.src.Repositories
{
    public class UserRepository : IUserRepository
    {

        private SupabaseContext _dbcontext;

        public UserRepository(SupabaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> ExistsAsync(Expression<Func<UserProfile, bool>> predicate)
        {
            var result = await _dbcontext.User.AnyAsync(predicate);
            return result;
        }

        public async Task<UserProfile?> GetByConditionAsync(Expression<Func<UserProfile, bool>> predicate)
        {
            return await _dbcontext.User.FirstOrDefaultAsync(predicate);
        }
    }
}