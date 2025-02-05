using Microsoft.AspNetCore.Mvc;
using HobbyCom.Application.src.DTOs.UserDTOs;
using HobbyCom.Application.src.IServices;

namespace HobbyCom.Presenter.API.src.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]s")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// <param name="createUserDTO">User registration details</param>
        /// <returns>Created user information</returns>
        /// <response code="201">User successfully created</response>
        /// <response code="400">Invalid user details</response>
        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetUserInfoDTO>> Create([FromBody] CreateUserDTO createUserDTO)
        {
            Console.WriteLine($"Received DTO: {System.Text.Json.JsonSerializer.Serialize(createUserDTO)}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                Console.WriteLine($"Validation Errors: {string.Join(", ", errors)}");
                return BadRequest(new { success = false, errors });
            }

            var createdUser = await _userService.CreateAsync(createUserDTO);
            return CreatedAtAction(nameof(Create), new { id = createdUser.Id }, new { success = true, data = createdUser });
        }
    }
}