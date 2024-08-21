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
                    UserId = Guid.NewGuid().ToString(),
                    Role = UserRole.User
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

    public class UserService_LoginShould
    {
        [Fact]
        public void Login_ValidCredentials_ThenReturnUser()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            var testEmail = "aggie@tamu.edu";
            var testPassword = "veryStrongP@ssw0rd";
            List<User> users =
            [
                new()
                {
                    Email = testEmail,
                    HashedPassword = BC.HashPassword(testPassword),
                    UserId = Guid.NewGuid().ToString(),
                    Role = UserRole.User
                }
            ];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            var userService = new UserService(mockUserRepository.Object);

            var user = userService.Login(testEmail, testPassword);

            Assert.Equivalent(users[0], user);
        }

        [Fact]
        public void Login_UnknownEmail_ThenArgumentException()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            var testEmail = "aggie@tamu.edu";
            var testPassword = "veryStrongP@ssw0rd";
            List<User> users = [];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            var userService = new UserService(mockUserRepository.Object);

            void action() => userService.Login(testEmail, testPassword);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email not registered!", ae.Message);
        }

        [Fact]
        public void Login_WrongPassword_ThenArgumentException()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            var testEmail = "aggie@tamu.edu";
            var testPassword = "veryStrongP@ssw0rd";
            List<User> users =
            [
                new()
                {
                    Email = testEmail,
                    HashedPassword = BC.HashPassword(testPassword),
                    UserId = Guid.NewGuid().ToString(),
                    Role = UserRole.User
                }
            ];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            var userService = new UserService(mockUserRepository.Object);

            void action() => userService.Login(testEmail, "someR@nd0mp@ssw0rd");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Wrong password!", ae.Message);
        }
    }

    public class UserService_GetUsersShould
    {
        [Fact]
        public void GetUsers_ThenReturnAllUsers()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users =
            [
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Role = UserRole.Admin
                },
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("awesomeStr0ngP@ssw0rd"),
                    Role = UserRole.User
                }
            ];
            mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            var userService = new UserService(mockUserRepository.Object);

            var returnedUsers = userService.GetUsers();

            Assert.Equivalent(users, returnedUsers);
        }
    }

    public class UserService_GetUserByIdShould
    {
        [Fact]
        public void GetUserById_GoodId_ThenReturnUser()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users =
            [
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Role = UserRole.Admin
                },
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("awesomeStr0ngP@ssw0rd"),
                    Role = UserRole.User
                }
            ];
            mockUserRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns((string userId) => users.Find(u => u.UserId == userId));
            var userService = new UserService(mockUserRepository.Object);

            var testEmail = users[0].UserId;
            var user = userService.GetUserById(testEmail);

            Assert.Equivalent(users[0], user);
        }

        [Fact]
        public void GetUserById_UnknownId_ThenArgumentException()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users =
            [
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Role = UserRole.Admin
                },
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("awesomeStr0ngP@ssw0rd"),
                    Role = UserRole.User
                }
            ];
            mockUserRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns((string userId) => users.Find(u => u.UserId == userId));
            var userService = new UserService(mockUserRepository.Object);

            void action() => userService.GetUserById(Guid.NewGuid().ToString());

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("User not found", ae.Message);
        }
    }

    public class UserService_UpdateUserShould
    {
        [Fact]
        public void UpdateUser_MutableAttributes_ThenUpdate()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users =
            [
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Role = UserRole.Admin
                },
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("awesomeStr0ngP@ssw0rd"),
                    Role = UserRole.User
                }
            ];
            mockUserRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns((string userId) => users.Find(u => u.UserId == userId));
            mockUserRepository
                .Setup(x => x.Update(It.IsAny<User>()))
                .Callback(
                    (User newUser) =>
                    {
                        var existingUserIdx = users.FindIndex(u => u.UserId == newUser.UserId);
                        users[existingUserIdx] = newUser;
                    }
                );
            var userService = new UserService(mockUserRepository.Object);
            var oldAdminCopy = new User()
            {
                UserId = users[0].UserId,
                Email = users[0].Email,
                HashedPassword = users[0].HashedPassword,
                Role = users[0].Role
            };
            var oldUserCopy = new User()
            {
                UserId = users[1].UserId,
                Email = users[1].Email,
                HashedPassword = users[1].HashedPassword,
                Role = users[1].Role
            };

            var newUser = new User()
            {
                UserId = oldUserCopy.UserId,
                Email = oldUserCopy.Email,
                HashedPassword = oldUserCopy.HashedPassword,
                Role = UserRole.Admin
            };
            userService.UpdateUser(newUser);

            Assert.Equal(2, users.Count);
            Assert.Equivalent(oldAdminCopy, users[0]);
            Assert.Equal(oldUserCopy.UserId, users[1].UserId);
            Assert.Equal(oldUserCopy.Email, users[1].Email);
            Assert.Equal(oldUserCopy.HashedPassword, users[1].HashedPassword);
            Assert.Equal(newUser.Role, users[1].Role);
        }

        [Fact]
        public void UpdateUser_TryUpdateImmutableAttributes_ThenNotUpdated()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users =
            [
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Role = UserRole.Admin
                },
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("awesomeStr0ngP@ssw0rd"),
                    Role = UserRole.User
                }
            ];
            mockUserRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns((string userId) => users.Find(u => u.UserId == userId));
            mockUserRepository
                .Setup(x => x.Update(It.IsAny<User>()))
                .Callback(
                    (User newUser) =>
                    {
                        var existingUserIdx = users.FindIndex(u => u.UserId == newUser.UserId);
                        users[existingUserIdx] = newUser;
                    }
                );
            var userService = new UserService(mockUserRepository.Object);
            var oldAdminCopy = new User()
            {
                UserId = users[0].UserId,
                Email = users[0].Email,
                HashedPassword = users[0].HashedPassword,
                Role = users[0].Role
            };
            var oldUserCopy = new User()
            {
                UserId = users[1].UserId,
                Email = users[1].Email,
                HashedPassword = users[1].HashedPassword,
                Role = users[1].Role
            };

            var newUser = new User()
            {
                UserId = oldUserCopy.UserId,
                Email = "aggie123@tamu.edu",
                HashedPassword = BC.HashPassword("newp@ssW0rd"),
                Role = oldUserCopy.Role
            };
            userService.UpdateUser(newUser);

            Assert.Equal(2, users.Count);
            Assert.Equivalent(oldAdminCopy, users[0]);
            Assert.Equal(oldUserCopy.UserId, users[1].UserId);
            Assert.Equal(oldUserCopy.Email, users[1].Email);
            Assert.Equal(oldUserCopy.HashedPassword, users[1].HashedPassword);
            Assert.Equal(oldUserCopy.Role, users[1].Role);
        }

        [Fact]
        public void UpdateUser_UnknownUser_ThenArgumentException()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            List<User> users =
            [
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Role = UserRole.Admin
                },
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("awesomeStr0ngP@ssw0rd"),
                    Role = UserRole.User
                }
            ];
            mockUserRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns((string userId) => users.Find(u => u.UserId == userId));
            var userService = new UserService(mockUserRepository.Object);

            var newUser = new User()
            {
                UserId = Guid.NewGuid().ToString(),
                Email = users[1].Email,
                HashedPassword = users[1].HashedPassword,
                Role = users[1].Role
            };
            void action() => userService.UpdateUser(newUser);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("User not found", ae.Message);
        }
    }

    public class UserService_DeleteUserShould
    {
        [Fact]
        public void DeleteUser_UserFound_ThenDeleted()
        {
            throw new Exception("TODO: Test not implemented yet!");
        }

        [Fact]
        public void DeleteUser_UserNotFound_ThenArgumentException()
        {
            throw new Exception("TODO: Test not implemented yet!");
        }
    }
}
