using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using UserAuthApi.Models;
using UserAuthApi.Services;

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public RegisterController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User registerUser)
        {
            // Check if the user already exists
            var existingUser = await _userService.GetUserByUsernameAsync(registerUser.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Create a new user model
            var newUser = new User
            {
                Username = registerUser.Username,
                Password = registerUser.Password, // Password will be hashed below
                Role = registerUser.Role
            };

            // Hash the password
            var passwordHasher = new PasswordHasher<User>();
            newUser.Password = passwordHasher.HashPassword(newUser, newUser.Password);

            // Add the user to the database
            await _userService.AddUserAsync(newUser);

            return Ok(new { message = "User registered successfully" });
        }
    }
}
