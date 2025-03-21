using Microsoft.AspNetCore.Mvc;
using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;
using System.Security.Claims;
using HobbyCom.Presenter.API.src.Services;
using Microsoft.AspNetCore.Authorization;
using HobbyCom.Application.src.DTOs.TokenDTOs;

namespace HobbyCom.Presenter.API.src.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]s")]
    // [Route("/api/v1/authentications")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISessionService _sessionService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            IUserService userService,
            ISessionService sessionService,
            IRefreshTokenService refreshTokenService
            )
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _sessionService = sessionService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetUserSessionDTO>> Create([FromBody] CreateUserDTO createUserDTO)
        {
            // if (!ModelState.IsValid)
            // {
            //     var errors = ModelState
            //         .Where(x => x.Value?.Errors.Count > 0)
            //         .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
            //         .ToList();
            //     return BadRequest(new { success = false, errors });
            // }

            var createdUser = await _authenticationService.CreateAsync(createUserDTO);
            return CreatedAtAction(
                nameof(Create),
                new { id = createdUser.Id },
                new { success = true, data = createdUser }
                );
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetUserSessionDTO>> Login([FromBody] LoginUserDTO loginUserDTO)
        {

            var loggedInUser = await _authenticationService.AuthenticateAsync(loginUserDTO);
            return Ok(new { success = true, data = loggedInUser });
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Logout([FromBody] LogoutDTO dto)
        {
            Console.WriteLine("1");
            var identity = User.Identity as ClaimsIdentity;

            // Get claims directly from the validated token
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;
            var userId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sessionId = identity?.FindFirst("SessionId")?.Value;

            // Validate required claims
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionId))
                return Unauthorized(new { success = false, error = "Invalid token claims" });

            // Verify email matches DTO
            if (dto.Email != email)
                return BadRequest(new { success = false, error = "Email mismatch" });

            // Revoke session
            var revokeResult = await _authenticationService.RevokeASession(
                Guid.Parse(userId),
                Guid.Parse(sessionId),
                dto.Refresh_token!
            );

            return revokeResult
                ? Ok(new { success = true, data = "Logged out successfully" })
                : BadRequest(new { success = false, error = "Failed to revoke session" });
        }

        /// <summary>
        /// Fetches a new access token and refresh token using the refresh token
        /// </summary>
        /// <returns>Task</returns>
        /// <response code="200">Token successfully refreshed</response>
        /// <response code="401">Invalid refresh token</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TokenDTO>> Refresh([FromBody] TokenDTO tokens)
        {
            if (string.IsNullOrEmpty(tokens.RefreshToken))
                return Unauthorized();

            var principal = _authenticationService.GetPrincipalFromExpiredToken(tokens.AccessToken!);
            if (principal?.Identity is not ClaimsIdentity identity)
                return Unauthorized();

            var userId = Guid.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var sessionId = Guid.Parse(identity.FindFirst("SessionId")!.Value);
            var email = identity.FindFirst(ClaimTypes.Email)!.Value;
            var role = identity.FindFirst(ClaimTypes.Role)!.Value;

            var userEmail = await _userService.GetUserEmailByIdAsync(userId);
            if (userEmail != tokens.Email || userEmail != email)
                return Unauthorized();

            // Validate session
            var session = await _sessionService.GetSessionByIdAndUserIdAsync(sessionId, userId);
            if (session == null)
                return Unauthorized();

            // Validate refresh token
            var refreshToken = await _refreshTokenService.GetRefreshTokenByTokenAsync(tokens.RefreshToken, userId, sessionId);
            if (refreshToken == null || refreshToken.TokenRevoked)
                return Unauthorized();

            // Generate new tokens
            var newAccessToken = await _authenticationService.GenerateAccessToken(userId, sessionId, email, role);
            var newRefreshToken = _authenticationService.GenerateRefreshToken();

            // Revoke old refresh token
            refreshToken.TokenRevoked = true;
            await _refreshTokenService.UpdateRefreshTokenAsync(refreshToken.Id, new UpdateTokenDTO { TokenRevoked = true });

            // Store new refresh token
            await _refreshTokenService.CreateRefreshTokenAsync(new CreateTokenDTO
            {
                UserId = session.UserId.ToString(),
                Token = newRefreshToken,
                CreatedAt = DateTime.UtcNow,
                TokenRevoked = false,
                SessionId = session.Id
            });

            return Ok(new { success = true, data = new { accessToken = newAccessToken, refreshToken = newRefreshToken, email = tokens.Email } });
        }
    }
}