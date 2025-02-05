using HobbyCom.Application.src.DTOs.UserDTOs;

namespace HobbyCom.Application.src.IServices
{
    public interface IUserService
    {
        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// <param name="createUserDTO">User registration details</param>
        /// <returns>Created user information</returns>
        Task<GetUserInfoDTO> CreateAsync(CreateUserDTO createUserDTO);
    }
}