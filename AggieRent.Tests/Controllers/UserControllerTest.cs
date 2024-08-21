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
                    UserId = Guid.NewGuid().ToString(),
                    Role = UserRole.User
                }
            ];
            mockUserService.Setup(x => x.GetUsers()).Returns(testUsers);
            var userController = new UserController(mockUserService.Object);

            var response = userController.GetAllUsers();
            var castResponse = Assert.IsType<OkObjectResult>(response);
            Assert.Equivalent(testUsers.Select(u => new UserSummaryDTO(u)), castResponse.Value);
        }
    }
}
