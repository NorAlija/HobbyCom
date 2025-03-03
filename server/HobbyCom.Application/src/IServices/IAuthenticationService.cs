using HobbyCom.Application.src.DTOs.UserDTOs;
using System.Security.Claims;

namespace HobbyCom.Application.src.IServices
{
    public interface IAuthenticationService
    {
        Task<GetUserInfoDTO> CreateAsync(CreateUserDTO createUserDTO);

        Task<GetUserInfoDTO> LoginAsync(LoginUserDTO loginUserDTO);

        Task<TokenDTO> RefreshTokenAsync(string refreshToken);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        Task<bool> LogoutAsync(string email);
    }
}