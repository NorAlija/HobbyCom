using HobbyCom.Application.src.DTOs.UserDTOs;
using System.Security.Claims;

namespace HobbyCom.Application.src.IServices
{
    public interface IAuthenticationService
    {
        Task<UserDTO> CreateAsync(CreateUserDTO createUserDTO);
        Task<UserDTO> AuthenticateAsync(LoginUserDTO loginUserDTO);
        Task<string> GenerateAccessToken(Guid userId, Guid sessionId, string email, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> RevokeAllSessions(Guid id);
        Task<bool> RevokeASession(Guid id, Guid sessionId, string refreshToken);
        Task<UserDTO> CreateUserSessionAsync(UserDTO user);
    }
}