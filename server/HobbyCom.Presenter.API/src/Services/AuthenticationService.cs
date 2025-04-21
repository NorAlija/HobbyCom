using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HobbyCom.Application.src.DTOs.SessionDTOs;
using HobbyCom.Application.src.DTOs.TokenDTOs;
using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;
using HobbyCom.Domain.src.Entities;
using HobbyCom.Domain.src.IRepositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Supabase;

namespace HobbyCom.Presenter.API.src.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPaswdService _paswdService;
        private readonly Client _supabaseClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IJwtRsaKeysService _jwtRsaKeysService;

        public AuthenticationService(
            IUserRepository userRepository,
            IPaswdService paswdService,
            Client supabaseClient,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IUserService userService,
            ISessionService sessionService,
            IRefreshTokenService refreshTokenService,
            IJwtRsaKeysService jwtRsaKeysService
        )
        {
            _userRepository = userRepository;
            _paswdService = paswdService;
            _supabaseClient = supabaseClient;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _userService = userService;
            _sessionService = sessionService;
            _refreshTokenService = refreshTokenService;
            _jwtRsaKeysService = jwtRsaKeysService;
        }

        public async Task<UserDTO> CreateAsync(CreateUserDTO createUserDTO)
        {
            var user = createUserDTO.ToEntity();
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Password cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username cannot be null or empty.");


            var usernameExists = await _userRepository.ExistsAsync(u => u.Username == user.Username);
            if (usernameExists)
            {
                throw new ArgumentException("Username is already taken.");
            }

            var emailExists = await _userRepository.ExistsAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                throw new ArgumentException("Email is already taken.");
            }

            var hashedPaswd = _paswdService.HashPaswd(user.Password!);
            // Check if confirm password matches the password
            var verifyPaswd = _paswdService.VerifyPaswd(createUserDTO.ConfirmPassword!, hashedPaswd);
            if (!verifyPaswd)
            {
                throw new Exception("Password and confirm password do not match.");
            }
            user.Password = hashedPaswd;
            // Set CreatedAt and UpdatedAt
            user.Created_At = DateTime.UtcNow;
            user.Updated_At = DateTime.UtcNow;



            var createdUser = await _userRepository.AddAsync(user); // how would look if we were using own database
            if (createdUser == null)
            {
                throw new Exception("Failed to create user in database.");
            }

            var userId = createdUser?.Id != null
                ? createdUser.Id
                : throw new Exception("User ID is null");

            var loginCredentials = new LoginUserDTO
            {
                Email = createdUser.Email,
                Password = createUserDTO.Password
            };

            var loggedInUser = await AuthenticateAsync(loginCredentials);
            return loggedInUser;
        }


        public async Task<UserDTO> AuthenticateAsync(LoginUserDTO loginUserDTO)
        {
            if (string.IsNullOrEmpty(loginUserDTO.Email))
            {
                throw new ArgumentNullException(nameof(loginUserDTO.Email));
            }
            if (string.IsNullOrEmpty(loginUserDTO.Password))
            {
                throw new ArgumentNullException(nameof(loginUserDTO.Password));
            }

            var user = await _userRepository.GetByConditionAsync(x => x.Email!.ToLower() == loginUserDTO.Email.ToLower());
            if (user == null)
            {
                throw new ArgumentException("Invalid Credentials");
            }

            // Verify password
            if (user.Password == null)
            {
                throw new ArgumentNullException(nameof(user.Password));
            }
            var isPasswordValid = _paswdService.VerifyPaswd(loginUserDTO.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new ArgumentException("Invalid Credentials");
            }

            // convert user to UserDTO
            var userDTO = new UserDTO().FromEntity(user);

            // does the user have a session
            var sessionExists = await _sessionService.CheckUserSessionExistAsync(user.Id);
            if (!sessionExists)
            {
                return await CreateUserSessionAsync(userDTO);
            }
            // if the user has a session in other devices
            return await CreateUserSessionAsync(userDTO);
        }


        public Task<string> GenerateAccessToken(Guid user_id, Guid session_id, string Email, string Role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user_id.ToString()),
                new Claim("SessionId", session_id.ToString()),
                new(ClaimTypes.Email, Email!),
                new(ClaimTypes.Role, Role!),
            };

            var privateKey = _jwtRsaKeysService.SigningKey;
            if (privateKey.KeySize < 2048)
                throw new CryptographicException("Insecure key size");

            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(privateKey),
                SecurityAlgorithms.RsaSha256);

            var tokenOptions = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Expires = DateTime.UtcNow.AddHours(1),
                // Expires = DateTime.UtcNow.AddMinutes(1),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = signingCredentials,
                Issuer = _configuration["Authentication:ValidIssuer"],
                Audience = _configuration["Authentication:ValidAudience"],
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.CreateToken(tokenOptions);

            var jwt = (JwtSecurityToken)accessToken;
            if (jwt.Header.Alg != SecurityAlgorithms.RsaSha256)
                throw new SecurityTokenException("Invalid algorithm");

            return Task.FromResult(tokenHandler.WriteToken(accessToken));
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[256];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(_jwtRsaKeysService.ValidationKey)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }


        public async Task<UserDTO> CreateUserSessionAsync(UserDTO user)
        {
            if (user.Email == null)
            {
                throw new ArgumentNullException(nameof(user.Email));
            }

            // check user exists
            var userProfile = await _userService.GetUserByEmailAsync(user.Email);
            if (userProfile.Email == null)
            {
                throw new ArgumentNullException("User not found");
            }

            if (userProfile.Role == null)
            {
                throw new ArgumentNullException("User role is not set");
            }

            var session = await _sessionService.CreateSessionAsync(new CreateSessionDTO
            {
                UserId = userProfile.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefreshedAt = DateTime.UtcNow
            });

            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            var refreshToken = GenerateRefreshToken();

            var presistRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(new CreateTokenDTO
            {
                UserId = session.UserId.ToString(),
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                TokenRevoked = false,
                SessionId = session.Id
            });

            if (presistRefreshToken == null)
            {
                throw new ArgumentNullException(nameof(presistRefreshToken));
            }

            var accessToken = await GenerateAccessToken(session.UserId, session.Id, userProfile.Email, userProfile.Role);

            //Build the response
            var userDTO = userProfile;
            var sessionDTO = session;
            var tokenDTO = presistRefreshToken;

            tokenDTO.Access_token = accessToken;
            userDTO.Sessions = new List<GetSessionDTO> { sessionDTO };
            sessionDTO.Tokens = new List<GetTokenDTO> { tokenDTO };

            return userDTO; //now corresponds to the commented out code below
            // return new GetUserSessionDTO
            // {
            //     Id = user.Id,
            //     Email = user.Email,
            //     FirstName = user.FirstName,
            //     LastName = user.LastName,
            //     UserName = user.Username,
            //     PhoneNumber = user.Phone,
            //     ProfilePicture = user.AvatarUrl,
            //     Role = user.Role,
            //     CreatedAt = user.CreatedAt,
            //     UpdatedAt = user.UpdatedAt,
            //     Sessions = new List<GetSessionDTO>
            //     {
            //         new GetSessionDTO
            //         {
            //             Id = session.Id,
            //             UserId = session.UserId,
            //             CreatedAt = session.CreatedAt,
            //             UpdatedAt = session.UpdatedAt,
            //             RefreshedAt = session.RefreshedAt,
            //             Tokens = new List<GetTokenDTO>
            //             {
            //                 new GetTokenDTO
            //                 {
            //                     Id = presistRefreshToken.Id,
            //                     UserId = presistRefreshToken.UserId,
            //                     Token = presistRefreshToken.Token,
            //                     Access_token = accessToken,
            //                     CreatedAt = presistRefreshToken.CreatedAt,
            //                     TokenRevoked = presistRefreshToken.TokenRevoked,
            //                     SessionId = presistRefreshToken.SessionId
            //                 }
            //             }
            //         }
            //     },
            // };
        }


        public Task<bool> RevokeAllSessions(Guid id)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> RevokeASession(Guid id, Guid sessionId, string refreshToken)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException($"ID {nameof(id)} is empty");
            }
            if (sessionId == Guid.Empty)
            {
                throw new ArgumentNullException($"ID {nameof(sessionId)} is empty");
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            var session = await _sessionService.GetSessionByIdAndUserIdAsync(sessionId, id);
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            var token = await _refreshTokenService.GetRefreshTokenByTokenAsync(refreshToken, session.UserId, session.Id);

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.Token != refreshToken || token.UserId != id.ToString() || token.SessionId != sessionId || token.TokenRevoked)
            {
                throw new ArgumentException("Invalid token");
            }

            var removeSession = await _sessionService.DeleteSessionAsync(token.SessionId, Guid.Parse(token.UserId));
            if (!removeSession)
            {
                throw new ArgumentNullException(nameof(removeSession));
            }

            return true;
        }
    }
}
