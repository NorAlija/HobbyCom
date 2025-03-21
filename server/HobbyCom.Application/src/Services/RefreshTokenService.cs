using HobbyCom.Application.src.DTOs.TokenDTOs;
using HobbyCom.Application.src.IServices;
using HobbyCom.Domain.src.IRepositories;

namespace HobbyCom.Application.src.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefresh_TokenRepository _refresh_TokenRepository;

        public RefreshTokenService(IRefresh_TokenRepository refresh_TokenRepository)
        {
            _refresh_TokenRepository = refresh_TokenRepository;
        }

        public async Task<GetTokenDTO> CreateRefreshTokenAsync(CreateTokenDTO createTokenDTO)
        {
            if (createTokenDTO == null)
            {
                throw new ArgumentNullException("Token cannot be null");
            }

            var token = await _refresh_TokenRepository.AddAsync(createTokenDTO.ToEntity());

            if (token == null)
            {
                throw new ArgumentNullException("Refresh token could not be presisted");
            }

            return new GetTokenDTO().FromEntity(token);
        }

        public async Task<GetTokenDTO> GetRefreshTokenByTokenAsync(string token, Guid userId, Guid sessionId)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            var refreshToken = await _refresh_TokenRepository.GetByConditionAsync(x => x.Token == token && x.User_Id == userId.ToString() && x.Session_Id == sessionId);
            if (refreshToken == null)
            {
                throw new ArgumentNullException("Refresh token not found ");
            }
            return new GetTokenDTO().FromEntity(refreshToken);
        }

        public async Task UpdateRefreshTokenAsync(Guid id, UpdateTokenDTO updateTokenDTO)
        {
            if (updateTokenDTO == null)
            {
                throw new ArgumentNullException("Value cannot be null");
            }

            var entity = await _refresh_TokenRepository.GetByConditionAsync(x => x.Id == id);
            if (entity == null)
            {
                throw new ArgumentNullException("Refresh token not found");
            }

            updateTokenDTO.UpdateEntity(entity);
            await _refresh_TokenRepository.UpdateAsync(entity);
        }
    }
}