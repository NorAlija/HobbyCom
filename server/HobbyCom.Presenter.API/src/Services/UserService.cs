using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;
using HobbyCom.Domain.src.IRepositories;
using Supabase;

namespace HobbyCom.Presenter.API.src.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly Client _supabaseClient;

        public UserService(IUserRepository userRepository, Client supabaseClient)
        {
            _userRepository = userRepository;
            _supabaseClient = supabaseClient;
        }
        public async Task<GetUserInfoDTO> CreateAsync(CreateUserDTO createUserDTO)
        {
            if (string.IsNullOrWhiteSpace(createUserDTO.Email))
                throw new ArgumentException("Email cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(createUserDTO.Password))
                throw new ArgumentException("Password cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(createUserDTO.Username))
                throw new ArgumentException("Username cannot be null or empty.");

            // Validate unique username
            var usernameExists = await _userRepository.ExistsAsync(u => u.Username == createUserDTO.Username);
            if (usernameExists)
            {
                throw new ArgumentException("Username is already taken.");
            }

            // Create user in auth.users table
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

            try
            {
                // var createdUser = await _userRepository.CreateAsync(newUser); // how would look if we were using own database
                var authResponse = await _supabaseClient.Auth.SignUp(
                    Supabase.Gotrue.Constants.SignUpType.Email,
                    createUserDTO.Email,
                    createUserDTO.Password,
                    signUpOptions
                );

                if (authResponse?.User == null)
                {
                    throw new Exception("Failed to create user in auth.users table.");
                }

                var userId = authResponse.User.Id != null
                    ? Guid.Parse(authResponse.User.Id)
                    : throw new Exception("User ID is null");

                // Map to DTO and return
                return new GetUserInfoDTO
                {
                    Id = authResponse.User.Id != null ? Guid.Parse(authResponse.User.Id) : throw new Exception("User ID is null"),
                    FirstName = createUserDTO.Firstname,
                    LastName = createUserDTO.Lastname,
                    Email = createUserDTO.Email,
                    Username = createUserDTO.Username,
                    Phone = createUserDTO.Phone,
                    Type = "USER",
                    // AvatarUrl = $"https://ui-avatars.com/api/?name={createUserDTO.Firstname}+{createUserDTO.Lastname}&background=random",
                    AvatarUrl = signUpOptions.Data["avatar_url"].ToString(),
                    CreatedAt = DateTime.UtcNow // You can set this to the current time
                };
            }
            catch (HttpRequestException httpEx)
            {
                // Network-related errors
                Console.WriteLine($"Network Error: {httpEx.Message}");
                throw new ApplicationException("Network error occurred during signup.", httpEx);
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException supabaseEx)
            {
                // Supabase-specific authentication errors
                Console.WriteLine($"Supabase Auth Error: {supabaseEx.Message}");

                // Handle specific Supabase error codes
                if (supabaseEx.Message.Contains("email already in use"))
                {
                    throw new ArgumentException("Email is already registered.", nameof(createUserDTO.Email), supabaseEx);
                }

                throw new ApplicationException("Authentication service error.", supabaseEx);
            }
            catch (ArgumentException)
            {
                // Re-throw argument-related exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                // Catch-all for unexpected errors
                Console.WriteLine($"Unexpected Signup Error: {ex.Message}");
                Console.WriteLine($"Full Exception: {ex}");
                throw new ApplicationException("An unexpected error occurred during signup.", ex);
            }
        }
    }
}