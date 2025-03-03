using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;
using HobbyCom.Domain.src.IRepositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Supabase;

namespace HobbyCom.Presenter.API.src.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly Client _supabaseClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, Client supabaseClient, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _supabaseClient = supabaseClient;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<bool> LogoutAsync(string email)
        {
            var signedInUser = _supabaseClient.Auth.CurrentUser;

            if (signedInUser == null)
            {
                throw new Exception("User is not signed in.");
            }

            if (signedInUser.Email != email)
            {
                throw new Exception("User is not signed in with the provided email.");
            }

            Console.WriteLine("signedInUser: ", signedInUser.Email, "email: ", email);

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

            var expiresAt = authResponse.ExpiresAt();

            // Force UTC formatting (if not already UTC)
            if (expiresAt.Kind != DateTimeKind.Utc)
            {
                expiresAt = expiresAt.ToUniversalTime();
            }

            return new GetUserInfoDTO
            {
                Access_token = authResponse.AccessToken,
                Token_type = authResponse.TokenType,
                Expires_in = authResponse.ExpiresIn,
                Expires_at = expiresAt.ToString("o"),
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
                    CreatedAt = authResponse.User.CreatedAt
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

            var loginCredentials = new LoginUserDTO
            {
                Email = authResponse.User.Email,
                Password = createUserDTO.Password
            };

            var loggedInUser = await LoginAsync(loginCredentials);
            return loggedInUser;

            // var expiresAt = authResponse.ExpiresAt();

            // // Force UTC formatting (if not already UTC)
            // if (expiresAt.Kind != DateTimeKind.Utc)
            // {
            //     expiresAt = expiresAt.ToUniversalTime();
            // }

            // return new GetUserInfoDTO
            // {
            //     Access_token = authResponse.AccessToken,
            //     Token_type = authResponse.TokenType,
            //     Expires_in = authResponse.ExpiresIn,
            //     Expires_at = expiresAt.ToString("o"),
            //     Expired = authResponse.Expired(),
            //     Refresh_token = authResponse.RefreshToken,
            //     User = new UserDTO
            //     {
            //         Id = authResponse.User.Id != null ? Guid.Parse(authResponse.User.Id) : throw new Exception("User ID is null"),
            //         FirstName = authResponse.User.UserMetadata["first_name"]?.ToString(),
            //         LastName = authResponse.User.UserMetadata["last_name"]?.ToString(),
            //         Email = authResponse.User.Email,
            //         Username = authResponse.User.UserMetadata["username"]?.ToString(),
            //         Phone = authResponse.User.UserMetadata["phone"]?.ToString(),
            //         Type = authResponse.User.UserMetadata["type"]?.ToString(),
            //         AvatarUrl = authResponse.User.UserMetadata["avatar_url"].ToString(),
            //         CreatedAt = authResponse.User.CreatedAt
            //     }
            // };
        }

        public async Task<TokenDTO> RefreshTokenAsync(string refreshToken)
        {
            var supabaseUrl = _configuration["Supabase:Url"];
            var tokenEndpoint = $"{supabaseUrl?.TrimEnd('/')}/auth/v1/token?grant_type=refresh_token";
            using var httpClient = _httpClientFactory.CreateClient("SupabaseClient");

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(new { refresh_token = refreshToken }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync(tokenEndpoint, jsonContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to refresh token: {responseContent}");
            }

            var tokenResponse = JsonConvert.DeserializeObject<SupabaseTokenResponse>(responseContent);

            if (tokenResponse == null)
            {
                throw new Exception("Failed to deserialize token response");
            }

            return new TokenDTO
            {
                AccessToken = tokenResponse.access_token,
                RefreshToken = tokenResponse.refresh_token
            };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSecret = _configuration["Authentication:JwtSecret"] ?? throw new ArgumentNullException(nameof(_configuration), "JwtSecret cannot be null");
            var bytes = Encoding.UTF8.GetBytes(jwtSecret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _configuration["Authentication:ValidIssuer2"],
                ValidAudience = _configuration["Authentication:ValidAudience"],
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(bytes)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}