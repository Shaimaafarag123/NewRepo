using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserAuthApi.Controllers;
using UserAuthApi.Services;
using UserAuthApi.Models;

namespace UserAuthApi.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<UserService> _userServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            // Mock the UserService
            _userServiceMock = new Mock<UserService>();

            // Create UserController with mocked service
            _userController = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUser_ExistingUser_ReturnsOkObjectResult()
        {
            // Arrange
            var username = "testuser";
            var user = new User {  Username = username };
            _userServiceMock.Setup(service => service.GetUserByUsernameAsync(username))
                            .ReturnsAsync(user);

            // Act
            var result = await _userController.GetUser(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            
            Assert.Equal(user.Username, returnedUser.Username);
        }

        [Fact]
        public async Task GetUser_NonExistentUser_ReturnsNotFoundResult()
        {
            // Arrange
            var username = "nonexistentuser";
            _userServiceMock.Setup(service => service.GetUserByUsernameAsync(username))
                            .ReturnsAsync((User)null);

            // Act
            var result = await _userController.GetUser(username);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}