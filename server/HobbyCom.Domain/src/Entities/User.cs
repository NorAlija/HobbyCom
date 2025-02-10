using System.ComponentModel.DataAnnotations;

namespace HobbyCom.Domain.src.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; }

        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public string? Phone { get; set; }
        public string? Type { get; set; } = "USER";
        public required string AvatarUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}