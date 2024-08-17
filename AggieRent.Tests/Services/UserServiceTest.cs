using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;
using Moq;
using Xunit;

namespace AggieRent.Tests.Services
{
    public class UserService_RegisterShould
    {
        [Theory]
        [InlineData("aggie@tamu.edu", "VerysecretP@ssw0rd")]
        [InlineData("Aggie@tamu.edu", "VerysecretP@ssw0rd")]
        [InlineData("aggie@TAMU.edu", "VerysecretP@ssw0rd")]
        public void Register_GoodInput_ThenOk(string email, string password)
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
            userService.Register(email, password);

            // Assert
            Assert.Single(users);
            Assert.Equal(email.ToLower(), users[0].Email);
            Assert.True(BC.Verify(password, users[0].HashedPassword));
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
                    HashedPassword = BC.HashPassword("VerysecretP@ssw0rd"),
                    UserId = Guid.NewGuid().ToString()
                }
            ];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            var userService = new UserService(mockUserRepository.Object);

            // Act
            void action() => userService.Register(email, "VerysecretP@ssw0rd");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }
    }
}
