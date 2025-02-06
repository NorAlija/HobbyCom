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

    }
}