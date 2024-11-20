using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using UserAuthApi.Services;
using UserAuthApi.Models;

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {
            // Fetch the user from the database
            var user = await _userService.GetUserByUsernameAsync(loginUser.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Verify the password using PasswordHasher
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginUser.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Password is valid, proceed to authenticate the user (e.g., generate JWT token)
            var authResponse = await _authService.AuthenticateAsync(loginUser.Username, loginUser.Password);

            return Ok(authResponse); // Return the JWT token on successful login
        }

    }
}