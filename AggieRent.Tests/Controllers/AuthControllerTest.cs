using System.Security.Claims;
using AggieRent.Common;
using AggieRent.Controllers;
using AggieRent.DTOs;
using AggieRent.Models;
using AggieRent.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Npgsql.TypeMapping;
using Xunit;

namespace AggieRent.Tests.Controllers
{
    public class AuthController_RegisterApplicantShould
    {
        [Fact]
        public void RegisterApplicant_GoodInput_Then201Created()
        {
            var mockAuthService = new Mock<IAuthService>();
            var authController = new AuthController(mockAuthService.Object);

            var dto = new ApplicantRegistrationDTO(
                "aggie@tamu.edu",
                "veryStr0ngP@ssw0rd",
                "John",
                "Doe",
                Gender.Female,
                DateOnly.FromDateTime(DateTime.UtcNow),
                "Hi, I'm John Doe"
            );
            var response = authController.RegisterApplicant(dto);

            Assert.IsType<CreatedResult>(response);
            mockAuthService.Verify(
                service =>
                    service.RegisterApplicant(
                        dto.Email,
                        dto.Password,
                        dto.FirstName,
                        dto.LastName,
                        dto.Gender,
                        dto.Birthday,
                        dto.Description
                    ),
                Times.Once()
            );
        }

        [Fact]
        public void RegisterApplicant_ServiceArgumentException_Then400BadRequest()
        {
            var mockAuthService = new Mock<IAuthService>();
            string testErrorMessage = "Some error message";
            mockAuthService
                .Setup(x =>
                    x.RegisterApplicant(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Gender>(),
                        It.IsAny<DateOnly?>(),
                        It.IsAny<string?>()
                    )
                )
                .Throws(new ArgumentException(testErrorMessage));

            var authController = new AuthController(mockAuthService.Object);

            var dto = new ApplicantRegistrationDTO(
                "aggie@tamu.edu",
                "veryStr0ngP@ssw0rd",
                "John",
                "Doe",
                Gender.Female,
                DateOnly.FromDateTime(DateTime.UtcNow),
                "Hi, I'm John Doe"
            );
            var response = authController.RegisterApplicant(dto);

            var castResponse = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equivalent(new ErrorResponseDTO(testErrorMessage), castResponse.Value);
        }
    }

    public class AuthController_LoginShould
    {
        [Theory]
        [InlineData("applicant@tamu.edu", AuthController.UserType.Applicant)]
        [InlineData("owner@tamu.edu", AuthController.UserType.Owner)]
        [InlineData("admin@tamu.edu", AuthController.UserType.Admin)]
        public async void Login_GoodInput_Then200OkWithUserSignedIn(
            string email,
            AuthController.UserType userType
        )
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var applicant = new Applicant()
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                HashedPassword = BC.HashPassword("verysTR0NGp@SSW0RD"),
                FirstName = "John",
                LastName = "Doe",
            };
            var owner = new Owner()
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                HashedPassword = BC.HashPassword("verysTR0NGp@SSW0RD"),
                Name = "John Doe",
            };
            var admin = new Admin()
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                HashedPassword = BC.HashPassword("verysTR0NGp@SSW0RD"),
            };
            mockAuthService
                .Setup(x => x.LoginApplicant(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(applicant);
            mockAuthService
                .Setup(x => x.LoginOwner(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(owner);
            mockAuthService
                .Setup(x => x.LoginAdmin(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(admin);

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

            var authController = new AuthController(mockAuthService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var response = await authController.Login(
                new LoginDTO(email, "verysTR0NGp@SSW0RD"),
                userType
            );

            // Assert
            Assert.IsType<OkResult>(response);
            var expectedRole = userType switch
            {
                AuthController.UserType.Applicant => ApplicationConstants.UserRoleClaim.Applicant,
                AuthController.UserType.Owner => ApplicationConstants.UserRoleClaim.Owner,
                AuthController.UserType.Admin => ApplicationConstants.UserRoleClaim.Admin,
                _ => throw new Exception("Bad UserType used in this test"),
            };
            var claims = capturedClaimsPrincipal.Claims;
            Assert.Contains(
                claims,
                c => c.Type == ClaimTypes.Name && c.Value.Equals(email.ToLower())
            );
            Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == expectedRole);
        }

        [Theory]
        [InlineData(AuthController.UserType.Applicant)]
        [InlineData(AuthController.UserType.Owner)]
        [InlineData(AuthController.UserType.Admin)]
        public async void Login_AuthServiceArgumentException_ThenBadRequestWithMessage(
            AuthController.UserType userType
        )
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var testErrorMessage = "Some error message";
            mockAuthService
                .Setup(x => x.LoginApplicant(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ArgumentException(testErrorMessage));
            mockAuthService
                .Setup(x => x.LoginOwner(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ArgumentException(testErrorMessage));
            mockAuthService
                .Setup(x => x.LoginAdmin(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ArgumentException(testErrorMessage));
            var authController = new AuthController(mockAuthService.Object);

            // Act
            var response = await authController.Login(
                new LoginDTO("aggie@tamu.edu", "strongP@ssw0rd"),
                userType
            );

            // Assert
            var castResponse = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equivalent(new ErrorResponseDTO(testErrorMessage), castResponse.Value);
        }
    }

    public class AuthController_LogoutShould
    {
        [Fact]
        public async void Logout_Then200Ok()
        {
            var mockAuthService = new Mock<IAuthService>();
            // Arrange authentication service, which is implicitly required by the SignInAsync method
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
                .Returns(Task.CompletedTask);
            mockHttpContext
                .Setup(x => x.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationService.Object);

            var authController = new AuthController(mockAuthService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            var response = await authController.Logout();

            Assert.IsType<OkResult>(response);
        }
    }
}
