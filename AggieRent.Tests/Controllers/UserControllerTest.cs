using AggieRent.Controllers;
using AggieRent.DTOs;
using AggieRent.Models;
using AggieRent.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AggieRent.Tests.Controllers
{
    public class UserController_GetAllUsersShould
    {
        [Fact]
        public void GetAllUsers_Then200Ok()
        {
            var mockUserService = new Mock<IUserService>();
            List<User> testUsers =
            [
                new()
                {
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("verySecretP@ssw0rd"),
                    UserId = Guid.NewGuid().ToString()
                }
            ];
            mockUserService.Setup(x => x.GetUsers()).Returns(testUsers);
            var userController = new UserController(mockUserService.Object);

            var response = userController.GetAllUsers();
            var castResponse = Assert.IsType<OkObjectResult>(response);
            Assert.Equivalent(testUsers.Select(u => new UserSummaryDTO(u)), castResponse.Value);
        }
    }

    public class UserController_RegisterShould
    {
        [Fact]
        public void Register_GoodInput_Then200Ok()
        {
            var mockUserService = new Mock<IUserService>();
            var userController = new UserController(mockUserService.Object);

            var userRegisterDto = new UserRegisterDTO()
            {
                Email = "aggie@tamu.edu",
                Password = "verySecretP@ssw0rd"
            };
            var response = userController.Register(userRegisterDto);

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
            var userController = new UserController(mockUserService.Object);

            var userRegisterDto = new UserRegisterDTO()
            {
                Email = "aggie@tamu.edu",
                Password = "verySecretP@ssw0rd"
            };
            var response = userController.Register(userRegisterDto);

            var castResponse = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equivalent(new ErrorResponseBody(testErrorMessage), castResponse.Value);
        }
    }
}
