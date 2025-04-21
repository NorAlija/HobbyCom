using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.SessionDTOs
{
    public class CreateSessionDTO : BaseCreateDto<Session>
    {
        public Guid UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? RefreshedAt { get; set; }


        public override Session ToEntity()
        {
            return new Session
            {
                User_Id = UserId,
                Created_At = CreatedAt,
                Updated_At = UpdatedAt,
                Refreshed_At = RefreshedAt
            };
        }
    }
}