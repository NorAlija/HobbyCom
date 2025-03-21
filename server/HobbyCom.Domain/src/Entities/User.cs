using System.ComponentModel.DataAnnotations;

namespace HobbyCom.Domain.src.Entities
{
    public class UserProfile : BaseEntity
    {
        [Required]
        public string? Firstname { get; set; }
        [Required]
        public string? Lastname { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; } = "USER";
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? AvatarUrl { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }

        public ICollection<Session> Sessions { get; } = new List<Session>();
    }
}