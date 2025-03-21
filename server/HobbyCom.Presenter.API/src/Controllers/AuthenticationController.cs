using Microsoft.AspNetCore.Mvc;
using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;
using System.Security.Claims;
using HobbyCom.Presenter.API.src.Services;
using Microsoft.AspNetCore.Authorization;

namespace HobbyCom.Presenter.API.src.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]s")]
    // [Route("/api/v1/authentications")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly IUserService _userService;

        public AuthenticationController(IAuthenticationService authenticationService, IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetUserInfoDTO>> Create([FromBody] CreateUserDTO createUserDTO)
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
                new { id = createdUser.User?.Id },
                new { success = true, data = createdUser }
                );
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetUserInfoDTO>> Login([FromBody] LoginUserDTO loginUserDTO)
        {

            var loggedInUser = await _authenticationService.LoginAsync(loginUserDTO);
            return Ok(new { success = true, data = loggedInUser });
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO dto)
        {
            Console.WriteLine("Logout Email: " + dto.Email);
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { success = false, error = "Email is required" });
            }
            await _authenticationService.LogoutAsync(dto.Email);
            return Ok(new { success = true, data = "Successfully logged out" });
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
            var principal = _authenticationService.GetPrincipalFromExpiredToken(tokens.AccessToken!);

            if (principal?.Identity is not ClaimsIdentity identity)
                return Unauthorized();
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();
            var userId = Guid.Parse(userIdClaim.Value);
            if (userId == Guid.Empty)
                return Unauthorized();
            var userEmail = await _userService.GetUserEmailByIdAsync(userId);
            if (userEmail != tokens.Email)
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(tokens.RefreshToken))
            {
                return Unauthorized();
            }

            var refreshedUser = await _authenticationService.RefreshTokenAsync(tokens.RefreshToken);
            return Ok(new { success = true, data = refreshedUser });
        }
    }
}