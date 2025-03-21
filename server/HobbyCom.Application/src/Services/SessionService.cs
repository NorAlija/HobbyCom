using HobbyCom.Application.src.DTOs.SessionDTOs;
using HobbyCom.Application.src.IServices;
using HobbyCom.Domain.src.IRepositories;

namespace HobbyCom.Application.src.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<bool> CheckUserSessionExistAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id cannot be empty");
            }

            var session = await _sessionRepository.ExistsAsync(x => x.User_Id == userId);

            return session;

        }

        public async Task<GetSessionDTO> CreateSessionAsync(CreateSessionDTO createSessionDTO)
        {
            if (createSessionDTO == null)
            {
                throw new ArgumentNullException("Session cannot be null");
            }

            var session = await _sessionRepository.AddAsync(createSessionDTO.ToEntity());

            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            return new GetSessionDTO().FromEntity(session);
        }

        public async Task<GetSessionDTO> GetSessionByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id cannot be empty");
            }

            var session = await _sessionRepository.GetByConditionAsync(x => x.User_Id == userId);
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            return new GetSessionDTO().FromEntity(session);
        }

        public async Task<GetSessionDTO> GetSessionByIdAndUserIdAsync(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Session id cannot be empty");
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id cannot be empty");
            }
            var session = await _sessionRepository.GetByConditionAsync(x => x.Id == id && x.User_Id == userId);
            if (session == null)
            {
                throw new ArgumentNullException("Session not found");
            }
            return new GetSessionDTO().FromEntity(session);
        }

        public async Task<bool> DeleteSessionAsync(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Session id cannot be empty");
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id cannot be empty");
            }
            var result = await _sessionRepository.DeleteAsync(userId, id);
            if (!result)
            {
                throw new InvalidOperationException("Failed to delete session. It may no longer exist.");
            }
            return result;
        }
    }
}