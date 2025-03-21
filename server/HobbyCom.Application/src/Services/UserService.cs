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

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var user = await _userRepository.GetByConditionAsync(x => x.Email!.ToLower() == email.ToLower());
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            // return new UserDTO
            // {
            //     Id = user.Id,
            //     FirstName = user.Firstname,
            //     LastName = user.Lastname,
            //     Email = user.Email,
            //     Username = user.Username,
            //     Phone = user.Phone,
            //     Type = user.Type,
            //     Password = user.Password,
            //     AvatarUrl = user.AvatarUrl,
            //     CreatedAt = user.Created_At ?? DateTime.MinValue,
            //     UpdatedAt = user.Updated_At ?? DateTime.MinValue
            // };
            return new UserDTO().FromEntity(user);
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
                throw new ArgumentNullException("User email not found");
            }

            return userEmail!.Email!;
        }
    }
}