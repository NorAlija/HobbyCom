using HobbyCom.Domain.src.Entities;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using Microsoft.EntityFrameworkCore;

namespace HobbyCom.Infrastructure.src.Repositories
{
    public class Refresh_TokenRepository : BaseRepository<Refresh_Token>, IRefresh_TokenRepository
    {
        private SupabaseContext _dbcontext;

        public Refresh_TokenRepository(SupabaseContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Refresh_Token?> GetTokenByUserSessionActive(Guid userId, Guid sessionId, bool tokenNotRevoked)
        {
            if (userId == Guid.Empty || sessionId == Guid.Empty)
            {
                return null;
            }
            if (tokenNotRevoked == true)
            {
                return null;
            }

            var token = await _setDB.FirstOrDefaultAsync(t => t.User_Id == userId.ToString() && t.Session_Id == sessionId && t.Token_Revoked == tokenNotRevoked);

            if (token == null)
            {
                return null;
            }
            return token;
        }
    }
}