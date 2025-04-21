using HobbyCom.Application.src.DTOs.UserDTOs;

namespace HobbyCom.Application.src.IServices
{
    public interface IUserService
    {
        Task<string> GetUserEmailByIdAsync(Guid id);

        Task<UserDTO> GetUserByEmailAsync(String email);

    }
}