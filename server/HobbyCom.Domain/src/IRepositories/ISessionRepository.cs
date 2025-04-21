using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Domain.src.IRepositories
{
    public interface ISessionRepository : IBaseRepository<Session>
    {
        Task<bool> DeleteAsync(Guid userId, Guid sessionId);
        Task<Session?> GetSessionByIdAndUserId(Guid sessionId, Guid userId);
    }
}