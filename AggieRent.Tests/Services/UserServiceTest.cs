using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;
using Moq;
using Xunit;

namespace AggieRent.Tests.Services
{
    public class UserService_RegisterShould
    {
        [Fact]
        public void Register_GoodInput_ThenOk()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users = [];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            mockUserRepository
                .Setup(x => x.Add(It.IsAny<User>()))
                .Callback((User u) => users.Add(u));
            var userService = new UserService(mockUserRepository.Object);

            // Act
            string testEmail = "aggie@tamu.edu";
            string testPassword = "123456";
            userService.Register(testEmail, testPassword);

            // Assert
            Assert.Single(users);
            Assert.Equal(testEmail, users[0].Email);
            Assert.True(BC.Verify(testPassword, users[0].HashedPassword));
        }

        [Theory]
        [InlineData("aggie@tamu.edu")]
        [InlineData("Aggie@tamu.edu")]
        [InlineData("aggie@TAMU.edu")]
        public void Register_DuplicateEmail_ThenArgumentException(string email)
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users =
            [
                new()
                {
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("123456"),
                    UserId = Guid.NewGuid().ToString()
                }
            ];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            var userService = new UserService(mockUserRepository.Object);

            // Act
            void action() => userService.Register(email, "123456");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }
    }
}
