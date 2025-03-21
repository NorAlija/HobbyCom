using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.TokenDTOs
{
    public class CreateTokenDTO : BaseCreateDto<Refresh_Token>
    {
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool TokenRevoked { get; set; }
        public Guid SessionId { get; set; }

        public override Refresh_Token ToEntity()
        {
            return new Refresh_Token
            {
                User_Id = UserId,
                Token = Token,
                Created_At = CreatedAt,
                Token_Revoked = TokenRevoked,
                Session_Id = SessionId
            };
        }
    }
}