using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.TokenDTOs
{
    public class UpdateTokenDTO : BaseUpdateDto<Refresh_Token>
    {
        public bool TokenRevoked { get; set; }

        public override void UpdateEntity(Refresh_Token entity)
        {
            entity.Token_Revoked = TokenRevoked;
        }
    }
}