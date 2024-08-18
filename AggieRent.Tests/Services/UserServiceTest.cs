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
        [InlineData("admin@localhost", "SuperStrongP@ssw0rd")]
        [InlineData("Admin@[127.0.0.1]", "SuperStrongP@ssw0rd")]
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
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("aggie@")]
        [InlineData("@tamu.edu")]
        [InlineData("admin@[127.0.0.1")]
        public void Register_InvalidEmailFormat_ThenArgumentException(string email)
        {
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users = [];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            mockUserRepository
                .Setup(x => x.Add(It.IsAny<User>()))
                .Callback((User u) => users.Add(u));
            var userService = new UserService(mockUserRepository.Object);

            var password = "SuperStrongP@ssw0rd";
            void action() => userService.Register(email, password);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Invalid email format!", ae.Message);
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
            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email already in use!", ae.Message);
        }

        [Theory]
        [InlineData("aB1_")]
        [InlineData("abcdefgh")]
        [InlineData("a1b2c3d4")]
        [InlineData("a1b2c3d_")]
        [InlineData("superstrongp@ssw0rd")]
        [InlineData("SUPERSTRONGP@SSW0RD")]
        [InlineData("SuperStrongPassw0rd")]
        [InlineData("SuperStrongP@ssword")]
        public void Register_WeakPassword_ThenArgumentException(string password)
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users = [];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            var userService = new UserService(mockUserRepository.Object);

            // Assert
            void action() => userService.Register("aggie@tamu.edu", password);

            // Assert
            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal(
                "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number",
                ae.Message
            );
        }
    }
}
