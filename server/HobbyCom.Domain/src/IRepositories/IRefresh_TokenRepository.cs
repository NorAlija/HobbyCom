using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Domain.src.IRepositories
{
    public interface IRefresh_TokenRepository : IBaseRepository<Refresh_Token>
    {
        Task<Refresh_Token?> GetTokenByUserSessionActive(Guid userId, Guid sessionId, bool tokenNotRevoked);
    }
}