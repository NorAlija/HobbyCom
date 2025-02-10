using System.ComponentModel.DataAnnotations;

namespace HobbyCom.Application.src.DTOs.UserDTOs
{
    public class LoginUserDTO
    {
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required, MinLength(1)]
        public string? Password { get; set; }
    }
}