using System.Text;
using System.Threading.Tasks;
using UserAuthApi.Repositories;
using UserAuthApi.Models;
using Microsoft.AspNetCore.Identity;

namespace UserAuthApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task AddUserAsync(User user)
        {
            var passwordHasher = new PasswordHasher<User>();
            // Hash the password first
            var hashedPassword = passwordHasher.HashPassword(user, user.Password);

            // Store the hashed password
            user.Password = hashedPassword;

            await _userRepository.AddAsync(user);
        }
    }
}
