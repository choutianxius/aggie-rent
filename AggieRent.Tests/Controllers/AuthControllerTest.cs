using System.Security.Claims;
using AggieRent.Common;
using AggieRent.Controllers;
using AggieRent.DTOs;
using AggieRent.Models;
using AggieRent.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AggieRent.Tests.Controllers
{
    public class UserController_RegisterShould
    {
        [Fact]
        public void Register_GoodInput_Then200Ok()
        {
            var mockUserService = new Mock<IUserService>();
            var authController = new AuthController(mockUserService.Object);

            var userRegisterDto = new UserRegisterDTO()
            {
                Email = "aggie@tamu.edu",
                Password = "verySecretP@ssw0rd"
            };
            var response = authController.Register(userRegisterDto);

            Assert.IsType<OkResult>(response);
            mockUserService.Verify(
                service => service.Register(userRegisterDto.Email, userRegisterDto.Password),
                Times.Once()
            );
        }

        [Fact]
        public void Register_ServiceArgumentException_Then400BadRequest()
        {
            var mockUserService = new Mock<IUserService>();
            string testErrorMessage = "Some error message";
            mockUserService
                .Setup(x => x.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ArgumentException(testErrorMessage));
            var authController = new AuthController(mockUserService.Object);

            var userRegisterDto = new UserRegisterDTO()
            {
                Email = "aggie@tamu.edu",
                Password = "verySecretP@ssw0rd"
            };
            var response = authController.Register(userRegisterDto);

            var castResponse = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equivalent(new ErrorResponseDTO(testErrorMessage), castResponse.Value);
        }
    }

    public class UserController_LoginShould
    {
        [Theory]
        [InlineData("user@tamu.edu", UserRole.User)]
        [InlineData("admin@tamu.edu", UserRole.Admin)]
        public async void Login_GoodInput_Then200OkWithUserSignedIn(string email, UserRole role)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var testPassword = "veryStr0ngp@ssw0rd";
            var user = new User()
            {
                Email = email.ToLower(),
                HashedPassword = BC.HashPassword(testPassword),
                UserId = Guid.NewGuid().ToString(),
                Role = role
            };
            mockUserService
                .Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(user);

            // Arrange authentication service, which is implicitly required by the SignInAsync method
            var capturedClaimsPrincipal = new ClaimsPrincipal();
            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthenticationService = new Mock<IAuthenticationService>();
            var mockSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            mockAuthenticationService
                .Setup(x =>
                    x.SignInAsync(
                        It.IsAny<HttpContext>(),
                        It.IsAny<string>(),
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<AuthenticationProperties>()
                    )
                )
                .Callback<HttpContext, string, ClaimsPrincipal, AuthenticationProperties>(
                    (context, scheme, principal, properties) =>
                    {
                        capturedClaimsPrincipal = principal;
                    }
                )
                .Returns(Task.CompletedTask);
            mockHttpContext
                .Setup(x => x.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationService.Object);

            var authController = new AuthController(mockUserService.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = mockHttpContext.Object }
            };

            // Act
            var response = await authController.Login(
                new UserLoginDTO() { Email = email, Password = testPassword }
            );

            // Assert
            Assert.IsType<OkResult>(response);
            var expectedRole = role.Equals(UserRole.Admin)
                ? ApplicationConstants.UserRoleClaim.Admin
                : ApplicationConstants.UserRoleClaim.User;
            var claims = capturedClaimsPrincipal.Claims;
            Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == user.Email);
            Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == expectedRole);
        }

        [Fact]
        public async void Login_UserServiceArgumentException_ThenBadRequestWithMessage()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var testErrorMessage = "Some error message";
            mockUserService
                .Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ArgumentException(testErrorMessage));
            var authController = new AuthController(mockUserService.Object);

            // Act
            var response = await authController.Login(
                new UserLoginDTO() { Email = "aggie@tamu.edu", Password = "strongP@ssw0rd" }
            );

            // Assert
            var castResponse = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equivalent(new ErrorResponseDTO(testErrorMessage), castResponse.Value);
        }
    }
}
