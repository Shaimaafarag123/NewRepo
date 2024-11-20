using System.Collections.Generic;
using System.Threading.Tasks;
using UserAuthApi.Models;

namespace UserAuthApi.Services
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
    }
}
