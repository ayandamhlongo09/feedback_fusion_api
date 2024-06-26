using Microsoft.AspNetCore.Mvc;
using FeedbackFusionAPI.Context;
using FeedbackFusionAPI.Model;
using FeedbackFusionAPI.Services.Interfaces;

namespace FeedbackFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private IUserService _userService;

        public UserController(IUserService userService, MyDbContext context)
        {
            _context = context;
            _userService = userService;
        }

        //Login
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticateResponse>> LoginUser(LoginRequest model)
        {
            try
            {
                var response = await _userService.Authenticate(model);
                if (response == null)
                    return Unauthorized("Invalid username or password.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        //Register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(RegisterRequest model)
        {
            try
            {
                var updatedUser = await _userService.AddUser(model);
                if (updatedUser == null)
                    return BadRequest("Failed to register user.");

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // Check if user is signed in and able to make requests.
        [HttpGet("authenticate/{userId}")]
        public async Task<ActionResult<User>> GetUserById(string userId)
        {
            try
            {
                var user = await _userService.GetById(userId);
                if (user == null)
                    return NotFound($"User with ID '{userId}' not found.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // Get all admin users (for admin only)
        [HttpGet("getAllAdminUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllAdminUsers(string userId)
        {
            try
            {
                var user = await _userService.GetById(userId);
                if (user == null)
                    return NotFound($"User with ID '{userId}' not found.");

                if (user.Role != 2 && user.Role != 1)
                    return Unauthorized("Unauthorized access.");

                var users = await _userService.GetAll();
                if (users == null || !users.Any())
                    return NotFound("No users found.");

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        // DELETE: api/User/5
        [HttpDelete("{idToDelete}")]
        public async Task<ActionResult<bool>> DeleteUser([FromQuery] string userId, string idToDelete)

        {
            try
            {
                var success = await _userService.DeleteUserAsync(userId, idToDelete);
                if (!success)
                {
                    return NotFound($"User with ID '{idToDelete}' not found.");
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }
    }
}
