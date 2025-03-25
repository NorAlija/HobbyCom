using HobbyCom.Application.src.DTOs.SessionDTOs;
using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.UserDTOs
{
    public class UserDTO : BaseReadDto<UserProfile>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        // public string? Password { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<GetSessionDTO>? Sessions { get; set; }


        public override UserDTO FromEntity(UserProfile entity)
        {
            return new UserDTO
            {
                Id = entity.Id,
                FirstName = entity.Firstname,
                LastName = entity.Lastname,
                Email = entity.Email,
                Username = entity.Username,
                Phone = entity.Phone,
                Role = entity.Role,
                AvatarUrl = entity.AvatarUrl,
                CreatedAt = entity.Created_At ?? DateTime.MinValue,
                UpdatedAt = entity.Updated_At ?? DateTime.MinValue,
                Sessions = entity.Sessions.Select(session => new GetSessionDTO().FromEntity(session)).ToList()
            };
        }
    }
}