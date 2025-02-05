using System.Linq.Expressions;
using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Domain.src.IRepositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Check if a user exists based on a condition
        /// </summary>
        /// <param name="predicate">Condition to check</param>
        /// <returns>True if user exists, false otherwise</returns>
        Task<bool> ExistsAsync(Expression<Func<UserProfile, bool>> predicate);

        /// <summary>
        /// Get a user by a specific condition
        /// </summary>
        /// <param name="predicate">Condition to filter users</param>
        /// <returns>User matching the condition</returns>
        Task<UserProfile?> GetByConditionAsync(Expression<Func<UserProfile, bool>> predicate);
    }
}