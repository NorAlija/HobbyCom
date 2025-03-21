using HobbyCom.Application.src.DTOs.SessionDTOs;
using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.UserDTOs
{
    public class GetUserSessionDTO : BaseReadDto<UserProfile>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<GetSessionDTO>? Sessions { get; set; }

        public override GetUserSessionDTO FromEntity(UserProfile entity)
        {
            return new GetUserSessionDTO
            {
                Id = entity.Id,
                FirstName = entity.Firstname,
                LastName = entity.Lastname,
                Email = entity.Email,
                UserName = entity.Username,
                PhoneNumber = entity.Phone,
                ProfilePicture = entity.AvatarUrl,
                Role = entity.Role,
                CreatedAt = entity.Created_At,
                UpdatedAt = entity.Updated_At,
                Sessions = entity.Sessions.Select(session => new GetSessionDTO().FromEntity(session)).ToList()
            };
        }
    }
}