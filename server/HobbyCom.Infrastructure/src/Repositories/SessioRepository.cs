using HobbyCom.Domain.src.Entities;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using Microsoft.EntityFrameworkCore;

namespace HobbyCom.Infrastructure.src.Repositories
{
    public class SessioRepository : BaseRepository<Session>, ISessionRepository
    {
        private SupabaseContext _dbcontext;

        public SessioRepository(SupabaseContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid sessionId)
        {
            var session = await _setDB.FirstOrDefaultAsync(s => s.Id == sessionId && s.User_Id == userId);
            if (session == null)
            {
                return false;
            }
            _setDB.Remove(session);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Session?> GetSessionByIdAndUserId(Guid sessionId, Guid userId)
        {
            if (sessionId == Guid.Empty || userId == Guid.Empty)
            {
                return null;
            }

            var session = await _setDB.FirstOrDefaultAsync(s => s.Id == sessionId && s.User_Id == userId);
            if (session == null)
            {
                return null;
            }
            return session;
        }
    }
}