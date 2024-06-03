using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestSimetricaConsulting.Controllers.V1;
using TestSimetricaConsulting.Model;
using TestSimetricaConsulting.Service;

namespace UnitTestSimetricaConsulting
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ValidModel_ReturnsOk()
        {
            // Arrange
            var user = new RegisterUserRequest
            {
                Username = "testuser",
                Password = "testpassword",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };
            _authServiceMock.Setup(service => service.RegisterUser(It.IsAny<Domain.User>())).ReturnsAsync("Usuario registrado con éxito.");

            // Act
            var result = await _controller.Register(user);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Usuario registrado con éxito.", okResult.Value);
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var user = new RegisterUserRequest(); // Modelo inválido

            // Act
            var result = await _controller.Register(user);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var login = new LoginUserRequest { UserName = "testuser", Password = "testpassword" };
            var user = new Domain.User { Username = "testuser", Password = "testpassword" };
            var token = "testtoken";

            _authServiceMock.Setup(service => service.Authenticate(login.UserName, login.Password))
                .ReturnsAsync(user);
            _authServiceMock.Setup(service => service.GenerateToken(user)).Returns(token);

            // Act
            var result = await _controller.Login(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginUserResponse = Assert.IsType<LoginUserResponse>(okResult.Value);

            Assert.NotNull(loginUserResponse);
            Assert.Equal(token, loginUserResponse.Token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var login = new LoginUserRequest { UserName = "testuser", Password = "testpassword" };

            _authServiceMock.Setup(service => service.Authenticate(login.UserName, login.Password)).ReturnsAsync((Domain.User)null);

            // Act
            var result = await _controller.Login(login);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Register_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var user = new RegisterUserRequest { 
                Username = "testuser", 
                Password = "testpassword", 
                Email = "test@example.com", 
                FirstName = "Test", 
                LastName = "User" 
            };

            _authServiceMock
                .Setup(service => service
                    .RegisterUser(It.IsAny<Domain.User>()))
                    .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Register(user);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Login_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var login = new LoginUserRequest { UserName = "testuser", Password = "testpassword" };

            _authServiceMock
                .Setup(service => service
                    .Authenticate(login.UserName, login.Password))
                    .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Login(login);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    }
}
