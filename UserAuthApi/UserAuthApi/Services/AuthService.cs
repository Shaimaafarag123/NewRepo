using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using UserAuthApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace UserAuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;  // Service to interact with the user data
        private readonly IConfiguration _configuration;  // Configuration to read JWT settings

        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<AuthResponse> AuthenticateAsync(string username, string password)
        {
            // Fetch the user from the database
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
            {
                Console.WriteLine("User not found"); // Debug message
                return null; // Return null if user doesn't exist
            }

            // Verify the password using PasswordHasher
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

            if (result == PasswordVerificationResult.Failed)
            {
                Console.WriteLine("Password verification failed"); // Debug message
                return null; // Return null if password doesn't match
            }

            // Generate JWT token if password is valid
            var token = GenerateJwtToken(user);

            // Return the authentication response with the token
            return new AuthResponse
            {
                AccessToken = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            // Create JWT claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
