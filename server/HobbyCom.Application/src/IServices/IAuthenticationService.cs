using HobbyCom.Application.src.DTOs.UserDTOs;

namespace HobbyCom.Application.src.IServices
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// <param name="createUserDTO">User registration details</param>
        /// <returns>Created user information</returns>
        Task<GetUserInfoDTO> CreateAsync(CreateUserDTO createUserDTO);

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="loginUserDTO">User login details</param>
        /// <returns>Logged in user information</returns>
        Task<GetUserInfoDTO> LoginAsync(LoginUserDTO loginUserDTO);

        /// <summary>
        /// Logout a user
        /// </summary>
        /// <returns>True if user is logged out, false otherwise</returns>
        Task<bool> LogoutAsync();
    }
}