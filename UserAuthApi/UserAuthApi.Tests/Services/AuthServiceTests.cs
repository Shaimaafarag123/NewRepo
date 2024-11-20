using Xunit;
using Moq;
using UserAuthApi.Services;
using UserAuthApi.Repositories;
using Microsoft.Extensions.Configuration;
using UserAuthApi.Models;
using FluentAssertions;
using System.Threading.Tasks;

namespace UserAuthApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Mocking the JWT secret configuration
            _configurationMock.Setup(config => config["JwtConfig:Secret"])
                              .Returns("SuperSecretKeyForTestingPurposes");

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "hashedpassword" };
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("testuser"))
                               .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync("testuser", "hashedpassword");

            // Assert
            result.Should().NotBeNull();
           
            result.TokenType.Should().Be("Bearer");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("invaliduser"))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync("invaliduser", "wrongpassword");

            // Assert
            result.Should().BeNull();
        }
    }
}