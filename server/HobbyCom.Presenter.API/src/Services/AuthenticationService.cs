using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;
using HobbyCom.Domain.src.IRepositories;
using Supabase;

namespace HobbyCom.Presenter.API.src.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly Client _supabaseClient;

        public AuthenticationService(IUserRepository userRepository, Client supabaseClient)
        {
            _userRepository = userRepository;
            _supabaseClient = supabaseClient;
        }

        public async Task<bool> LogoutAsync()
        {
            await _supabaseClient.Auth.SignOut();
            return true;
        }

        public async Task<GetUserInfoDTO> LoginAsync(LoginUserDTO loginUserDTO)
        {
            if (string.IsNullOrWhiteSpace(loginUserDTO.Email))
                throw new ArgumentException("Email cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(loginUserDTO.Password))
                throw new ArgumentException("Password cannot be null or empty.");

            var authResponse = await _supabaseClient.Auth.SignIn(loginUserDTO.Email, loginUserDTO.Password);

            Console.WriteLine(authResponse?.ExpiresIn);

            if (authResponse?.User == null)
            {
                throw new Exception("Incorrect email or password.");
            }

            return new GetUserInfoDTO
            {
                Access_token = authResponse.AccessToken,
                Token_type = authResponse.TokenType,
                Expires_in = authResponse.ExpiresIn,
                Expires_at = authResponse.ExpiresAt().ToString("o"),
                Expired = authResponse.Expired(),
                Refresh_token = authResponse.RefreshToken,
                User = new UserDTO
                {
                    Id = authResponse.User.Id != null ? Guid.Parse(authResponse.User.Id) : throw new Exception("User ID is null"),
                    FirstName = authResponse.User.UserMetadata["first_name"]?.ToString(),
                    LastName = authResponse.User.UserMetadata["last_name"]?.ToString(),
                    Email = authResponse.User.Email,
                    Username = authResponse.User.UserMetadata["username"]?.ToString(),
                    Phone = authResponse.User.UserMetadata["phone"]?.ToString(),
                    Type = authResponse.User.UserMetadata["type"]?.ToString(),
                    AvatarUrl = authResponse.User.UserMetadata["avatar_url"].ToString(),
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public async Task<GetUserInfoDTO> CreateAsync(CreateUserDTO createUserDTO)
        {
            if (string.IsNullOrWhiteSpace(createUserDTO.Email))
                throw new ArgumentException("Email cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(createUserDTO.Password))
                throw new ArgumentException("Password cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(createUserDTO.Username))
                throw new ArgumentException("Username cannot be null or empty.");


            var usernameExists = await _userRepository.ExistsAsync(u => u.Username == createUserDTO.Username);
            if (usernameExists)
            {
                throw new ArgumentException("Username is already taken.");
            }

            var signUpOptions = new Supabase.Gotrue.SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "first_name", createUserDTO.Firstname ?? string.Empty },
                    { "last_name", createUserDTO.Lastname ?? string.Empty },
                    { "username", createUserDTO.Username ?? string.Empty },
                    { "phone", createUserDTO.Phone  ?? string.Empty },
                    { "type", createUserDTO.Type ?? "USER" },
                    { "avatar_url", $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(createUserDTO.Firstname ?? "")}" +
                             $"+{Uri.EscapeDataString(createUserDTO.Lastname ?? "")}&background=random" }
                }
            };

            // var createdUser = await _userRepository.CreateAsync(newUser); // how would look if we were using own database
            var authResponse = await _supabaseClient.Auth.SignUp(
                Supabase.Gotrue.Constants.SignUpType.Email,
                createUserDTO.Email,
                createUserDTO.Password,
                signUpOptions
            );

            // Console.WriteLine(authResponse?.User.);

            if (authResponse?.User == null)
            {
                throw new Exception("Failed to create user in auth.users table.");
            }

            var userId = authResponse.User.Id != null
                ? Guid.Parse(authResponse.User.Id)
                : throw new Exception("User ID is null");

            return new GetUserInfoDTO
            {
                Access_token = authResponse.AccessToken,
                Token_type = authResponse.TokenType,
                Expires_in = authResponse.ExpiresIn,
                Expires_at = authResponse.ExpiresAt().ToString("o"),
                Expired = authResponse.Expired(),
                Refresh_token = authResponse.RefreshToken,
                User = new UserDTO
                {
                    Id = authResponse.User.Id != null ? Guid.Parse(authResponse.User.Id) : throw new Exception("User ID is null"),
                    FirstName = authResponse.User.UserMetadata["first_name"]?.ToString(),
                    LastName = authResponse.User.UserMetadata["last_name"]?.ToString(),
                    Email = authResponse.User.Email,
                    Username = authResponse.User.UserMetadata["username"]?.ToString(),
                    Phone = authResponse.User.UserMetadata["phone"]?.ToString(),
                    Type = authResponse.User.UserMetadata["type"]?.ToString(),
                    AvatarUrl = authResponse.User.UserMetadata["avatar_url"].ToString(),
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}