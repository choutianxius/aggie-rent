using AggieRent.Controllers;
using AggieRent.DTOs;
using AggieRent.Services;
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
}
