using Moq;
using Xunit;
using System.Threading.Tasks;
using UserAuthApi.Models;
using UserAuthApi.Repositories;
using UserAuthApi.Services;
using Microsoft.AspNetCore.Identity;

namespace UserAuthApi.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Mock the IUserRepository
            _userRepositoryMock = new Mock<IUserRepository>();

            // Create UserService with mocked repository and password hasher
            //_userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var username = "testuser";
            var user = new User {  Username = username };
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
                               .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_NonExistentUser_ReturnsNull()
        {
            // Arrange
            var username = "nonexistentuser";
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddUserAsync_NewUser_AddsUserSuccessfully()
        {
            // Arrange
            var user = new User { Username = "newuser", Password = "password" };

            // Act
            await _userService.AddUserAsync(user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.Is<User>(u => u.Username == user.Username)), Times.Once);
        }
    }
}