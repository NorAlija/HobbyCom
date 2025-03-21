using HobbyCom.Application.src.DTOs.TokenDTOs;
using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.SessionDTOs
{
    public class GetSessionDTO : BaseReadDto<Session>
    {
        public Guid UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? RefreshedAt { get; set; }
        public ICollection<GetTokenDTO>? Tokens { get; set; }

        public override GetSessionDTO FromEntity(Session entity)
        {
            return new GetSessionDTO
            {
                Id = entity.Id,
                UserId = entity.User_Id,
                CreatedAt = entity.Created_At,
                UpdatedAt = entity.Updated_At,
                RefreshedAt = entity.Refreshed_At,
                Tokens = entity.Refresh_Tokens.Select(token => new GetTokenDTO().FromEntity(token)).ToList()
            };
        }
    }
}