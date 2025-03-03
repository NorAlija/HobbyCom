using HobbyCom.Application.src.DTOs.UserDTOs;

namespace HobbyCom.Application.src.IServices
{
    public interface IUserService
    {

        /// <summary>
        /// Get logged in user information
        /// </summary>
        /// <returns>Logged in user information</returns>
        Task<GetUserInfoDTO> GetLoggedInUserInfo();

        /// <summary>
        /// Get user email by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User email</returns>
        Task<string> GetUserEmailByIdAsync(Guid id);

    }
}