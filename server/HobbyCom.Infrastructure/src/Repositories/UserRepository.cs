using System.Linq.Expressions;
using HobbyCom.Domain.src.Entities;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using Microsoft.EntityFrameworkCore;

namespace HobbyCom.Infrastructure.src.Repositories
{
    public class UserRepository : BaseRepository<UserProfile>, IUserRepository
    {

        private SupabaseContext _dbcontext;

        public UserRepository(SupabaseContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }


    }
}