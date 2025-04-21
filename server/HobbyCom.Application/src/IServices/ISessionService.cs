using HobbyCom.Application.src.DTOs.SessionDTOs;

namespace HobbyCom.Application.src.IServices
{
    public interface ISessionService
    {
        Task<bool> CheckUserSessionExistAsync(Guid userId);
        Task<GetSessionDTO> CreateSessionAsync(CreateSessionDTO createSessionDTO);
        Task<GetSessionDTO> GetSessionByUserIdAsync(Guid userId);
        Task<GetSessionDTO> GetSessionByIdAndUserIdAsync(Guid id, Guid userId);
        Task<bool> DeleteSessionAsync(Guid id, Guid userId);

    }
}