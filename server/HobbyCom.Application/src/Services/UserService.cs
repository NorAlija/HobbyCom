using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;
using HobbyCom.Domain.src.IRepositories;

namespace HobbyCom.Application.src.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<GetUserInfoDTO> GetLoggedInUserInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserEmailByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException($"ID {nameof(id)} is empty");
            }

            var userEmail = await _userRepository.GetByConditionAsync(x => x.Id == id);
            if (userEmail == null)
            {
                throw new ArgumentNullException($"{nameof(userEmail)} is null");
            }

            return userEmail.Email;
        }
    }
}