using System.ComponentModel.DataAnnotations;
using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs.UserDTOs
{
    public class CreateUserDTO : BaseCreateDto<UserProfile>
    {
        [Required, MinLength(2, ErrorMessage = "Firstname must be at least 2 characters long.")]
        [MaxLength(50)]
        public string? Firstname { get; set; }

        [Required, MinLength(2, ErrorMessage = "Lastname must be at least 2 characters long.")]
        [MaxLength(50)]
        public string? Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required, MinLength(4, ErrorMessage = "Username must be at least 4 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username must contain only letters and numbers.")]
        public string? Username { get; set; }

        [MinLength(5, ErrorMessage = "Phone number can't be less than 5 digits")]
        [MaxLength(15, ErrorMessage = "Phone number can't exceed 15 digits")]
        [Phone]
        public string? Phone { get; set; }

        public string? Role { get; set; }

        public string? AvatarUrl { get; set; }

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }

        [Required, Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        public override UserProfile ToEntity()
        {
            return new UserProfile
            {
                Firstname = Firstname,
                Lastname = Lastname,
                Email = Email,
                Username = Username,
                Phone = Phone,
                Role = Role,
                AvatarUrl = AvatarUrl,
                Password = Password,
                Created_At = DateTime.UtcNow,
                Updated_At = DateTime.UtcNow
            };
        }
    }
}