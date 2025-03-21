using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.TokenDTOs
{
    public class GetTokenDTO : BaseReadDto<Refresh_Token>
    {
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool TokenRevoked { get; set; }
        public Guid SessionId { get; set; }
        public string? Access_token { get; set; }

        public override GetTokenDTO FromEntity(Refresh_Token entity)
        {
            return new GetTokenDTO
            {
                Id = entity.Id,
                UserId = entity.User_Id,
                Token = entity.Token,
                CreatedAt = entity.Created_At,
                TokenRevoked = entity.Token_Revoked,
                SessionId = entity.Session_Id
            };
        }
    }
}