using Microsoft.AspNetCore.Mvc;
using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;

namespace HobbyCom.Presenter.API.src.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly IUserService _userService;

        public AuthenticationController(IAuthenticationService authenticationService, IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// <param name="createUserDTO">User registration details</param>
        /// <returns>Created user information</returns>
        /// <response code="201">User successfully created</response>
        /// <response code="400">Invalid user details</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                new { id = createdUser.Id },
                new { success = true, data = createdUser }
                );
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="loginUserDTO">User login details</param>
        /// <returns>Logged in user information</returns>
        /// <response code="200">User successfully logged in</response>
        /// <response code="400">Invalid login details</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetUserInfoDTO>> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            // if (!ModelState.IsValid)
            // {
            //     var errors = ModelState
            //         .Where(x => x.Value?.Errors.Count > 0)
            //         .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
            //         .ToList();
            //     return BadRequest(new { success = false, errors });
            // }

            var loggedInUser = await _authenticationService.LoginAsync(loginUserDTO);
            return Ok(new { success = true, data = loggedInUser });
        }

        /// <summary>
        /// Logout a user
        /// </summary>
        /// <returns>Task</returns>
        /// <response code="200">User successfully logged out</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.LogoutAsync();
            return Ok(new { success = true, data = "Successfully logged out" });
        }
    }
}