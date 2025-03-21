using HobbyCom.Application.src.DTOs.TokenDTOs;

namespace HobbyCom.Application.src.IServices
{
    public interface IRefreshTokenService
    {
        Task<GetTokenDTO> CreateRefreshTokenAsync(CreateTokenDTO createTokenDTO);

        Task<GetTokenDTO> GetRefreshTokenByTokenAsync(string token, Guid userId, Guid sessionId);

        Task UpdateRefreshTokenAsync(Guid id, UpdateTokenDTO updateTokenDTO);

    }
}