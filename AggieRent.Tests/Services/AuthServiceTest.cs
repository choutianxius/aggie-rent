using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;
using Moq;
using Xunit;

namespace AggieRent.Tests.Services
{
    public class AuthService_RegisterApplicantShould
    {
        [Theory]
        [InlineData("aggie@tamu.edu")]
        [InlineData("Aggie@tamu.edu")]
        [InlineData("aggie@TAMU.edu")]
        [InlineData("applicant@localhost")]
        [InlineData("Appliant@[127.0.0.1]")]
        public void RegisterApplicant_GoodInput_ThenStoreNormalizedEmail(string email)
        {
            // Arrange
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Applicant> applicants = [];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            mockApplicantRepository
                .Setup(x => x.Add(It.IsAny<Applicant>()))
                .Callback((Applicant u) => applicants.Add(u));
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            // Act
            var testPassword = "SuperStrongP@ssw0rd";
            var testFirstName = "John";
            var testLastName = "Doe";
            var testGender = Gender.Female;
            var testBirthday = DateOnly.FromDateTime(DateTime.UtcNow);
            var testDescription = "Hello, I'm John Doe";
            authService.RegisterApplicant(
                email,
                testPassword,
                testFirstName,
                testLastName,
                testGender,
                testBirthday,
                testDescription
            );

            // Assert
            Assert.Single(applicants);
            Assert.Equal(email.ToLower(), applicants[0].Email);
            Assert.True(BC.Verify(testPassword, applicants[0].HashedPassword));
            Assert.Equal(testFirstName, applicants[0].FirstName);
            Assert.Equal(testLastName, applicants[0].LastName);
            Assert.Equal(testGender, applicants[0].Gender);
            Assert.Equal(testBirthday, applicants[0].Birthday);
            Assert.Equal(testDescription, applicants[0].Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("aggie@")]
        [InlineData("@tamu.edu")]
        [InlineData("admin@[127.0.0.1")]
        public void RegisterApplicant_InvalidEmailFormat_ThenArgumentException(string email)
        {
            // Arrange
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Applicant> applicants = [];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            mockApplicantRepository
                .Setup(x => x.Add(It.IsAny<Applicant>()))
                .Callback((Applicant u) => applicants.Add(u));
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            // Act
            var testPassword = "SuperStrongP@ssw0rd";
            var testFirstName = "John";
            var testLastName = "Doe";
            var testGender = Gender.Female;
            var testBirthday = DateOnly.FromDateTime(DateTime.UtcNow);
            var testDescription = "Hello, I'm John Doe";
            void action() =>
                authService.RegisterApplicant(
                    email,
                    testPassword,
                    testFirstName,
                    testLastName,
                    testGender,
                    testBirthday,
                    testDescription
                );

            // Assert
            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Invalid email format!", ae.Message);
        }

        [Theory]
        [InlineData("aggie@tamu.edu")]
        [InlineData("Aggie@tamu.edu")]
        [InlineData("aggie@TAMU.edu")]
        public void RegisterApplicant_DuplicateEmail_ThenArgumentException(string email)
        {
            // Arrange
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email.ToLower(),
                    HashedPassword = BC.HashPassword("SUperStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
            ];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            mockApplicantRepository
                .Setup(x => x.Add(It.IsAny<Applicant>()))
                .Callback((Applicant u) => applicants.Add(u));
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            // Act
            var testPassword = "SuperStrongP@ssw0rd";
            var testFirstName = "John";
            var testLastName = "Doe";
            var testGender = Gender.Female;
            var testBirthday = DateOnly.FromDateTime(DateTime.UtcNow);
            var testDescription = "Hello, I'm John Doe";
            void action() =>
                authService.RegisterApplicant(
                    email,
                    testPassword,
                    testFirstName,
                    testLastName,
                    testGender,
                    testBirthday,
                    testDescription
                );

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
        public void RegisterApplicant_WeakPassword_ThenArgumentException(string password)
        {
            // Arrange
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Applicant> applicants = [];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            mockApplicantRepository
                .Setup(x => x.Add(It.IsAny<Applicant>()))
                .Callback((Applicant u) => applicants.Add(u));
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            // Act
            var testEmail = "aggie@tamu.edu";
            var testFirstName = "John";
            var testLastName = "Doe";
            var testGender = Gender.Female;
            var testBirthday = DateOnly.FromDateTime(DateTime.UtcNow);
            var testDescription = "Hello, I'm John Doe";
            void action() =>
                authService.RegisterApplicant(
                    testEmail,
                    password,
                    testFirstName,
                    testLastName,
                    testGender,
                    testBirthday,
                    testDescription
                );

            // Assert
            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal(
                "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number",
                ae.Message
            );
        }
    }

    public class AuthService_LoginApplicantShould
    {
        [Fact]
        public void LoginApplicant_ValidCredentials_ThenReturnUser()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            var testEmail = "aggie@tamu.edu";
            var testPassword = "veryStrongP@ssw0rd";
            var testFirstName = "John";
            var testLastName = "Doe";
            var testGender = Gender.Female;
            var testBirthday = DateOnly.FromDateTime(DateTime.UtcNow);
            var testDescription = "Hello, I'm John Doe";
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = testEmail,
                    HashedPassword = BC.HashPassword(testPassword),
                    FirstName = testFirstName,
                    LastName = testLastName,
                    Gender = testGender,
                    Birthday = testBirthday,
                    Description = testDescription,
                },
            ];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            var applicant = authService.LoginApplicant(testEmail, testPassword);

            Assert.Equivalent(applicants[0], applicant);
        }

        [Fact]
        public void LoginApplicant_UnknownEmail_ThenArgumentException()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            var testEmail = "aggie@tamu.edu";
            var testPassword = "veryStrongP@ssw0rd";
            List<Applicant> applicants = [];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            void action() => authService.LoginApplicant(testEmail, testPassword);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email not registered!", ae.Message);
        }

        [Fact]
        public void Login_WrongPassword_ThenArgumentException()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            var testEmail = "aggie@tamu.edu";
            var testPassword = "veryStrongP@ssw0rd";
            var testFirstName = "John";
            var testLastName = "Doe";
            var testGender = Gender.Female;
            var testBirthday = DateOnly.FromDateTime(DateTime.UtcNow);
            var testDescription = "Hello, I'm John Doe";
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = testEmail,
                    HashedPassword = BC.HashPassword(testPassword),
                    FirstName = testFirstName,
                    LastName = testLastName,
                    Gender = testGender,
                    Birthday = testBirthday,
                    Description = testDescription,
                },
            ];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            void action() => authService.LoginApplicant(testEmail, "someR@nd0mp@ssw0rd");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Wrong password!", ae.Message);
        }
    }

    public class AuthService_LoginOwnerShould
    {
        Mock<IAdminRepository> mockAdminRepository = new Mock<IAdminRepository>();
        Mock<IApplicantRepository> mockApplicantRepository = new Mock<IApplicantRepository>();

        [Fact]
        public void LoginOwner_ValidCredentials_ThenReturnOwner()
        {
            var mockOwnerRepository = new Mock<IOwnerRepository>();

            var testEmail = "owner@niceestate.com";
            var testName = "Nice Estate";
            var testPassword = "Verystrongp@ssword";
            var testDescription = "Official account for apartment owner Nice Estate";

            List<Owner> owners =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = testEmail,
                    Name = testName,
                    HashedPassword = BC.HashPassword(testPassword),
                    Description = testDescription,
                },
            ];

            mockOwnerRepository.Setup(x => x.GetAll()).Returns(owners.AsQueryable());
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            var owner = authService.LoginOwner(testEmail, testPassword);
            Assert.Equivalent(owners[0], owner);
        }

        [Fact]
        public void LoginOwner_UnknownEmail_ThenArgumentException()
        {
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var testEmail = "unknownowner@unkownestate.com";
            var testPassword = "Strongp@ssword1";

            List<Owner> emptyOwnersList = [];
            mockOwnerRepository.Setup(x => x.GetAll()).Returns(emptyOwnersList.AsQueryable());

            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );
            // Capture the action of logging in
            void action() => authService.LoginOwner(testEmail, testPassword);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email not registered!", ae.Message);
        }

        [Fact]
        public void LoginOwner_WrongPassword_ThenArgumentException()
        {
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            var testEmail = "owner@niceestate.com";
            var testName = "Nice Estate";
            var testPassword = "Verystrongp@ssword";
            var wrongPassword = "Wrongp@ssword";
            var testDescription = "Official account for apartment owner Nice Estate";

            List<Owner> owners =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = testEmail,
                    Name = testName,
                    HashedPassword = BC.HashPassword(testPassword),
                    Description = testDescription,
                },
            ];

            mockOwnerRepository.Setup(x => x.GetAll()).Returns(owners.AsQueryable());
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );
            void action() => authService.LoginOwner(testEmail, wrongPassword);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Wrong password!", ae.Message);
        }
    }

    public class AuthService_LoginAdminShould
    {
        Mock<IApplicantRepository> mockApplicantRepository = new Mock<IApplicantRepository>();
        Mock<IOwnerRepository> mockOwnerRepository = new Mock<IOwnerRepository>();

        [Fact]
        public void LoginAdmin_ValidCredentials_ThenReturnAdmin()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();

            var testEmail = "admin@aggierent.com";
            var testPassword = "Verystrongp@ssword";

            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = testEmail,
                    HashedPassword = BC.HashPassword(testPassword),
                },
            ];

            mockAdminRepository.Setup(x => x.GetAll()).Returns(admins.AsQueryable());
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );

            var admin = authService.LoginAdmin(testEmail, testPassword);
            Assert.Equivalent(admins[0], admin);
        }

        [Fact]
        public void LoginAdmin_UnknownEmail_ThenArgumentException()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            var testEmail = "notadmin@notadminemail.com";
            var testPassword = "Strongp@ssword1";

            List<Admin> emptyAdminsList = [];
            mockAdminRepository.Setup(x => x.GetAll()).Returns(emptyAdminsList.AsQueryable());

            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );
            // Capture the action of logging in
            void action() => authService.LoginAdmin(testEmail, testPassword);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email not registered!", ae.Message);
        }

        [Fact]
        public void LoginAdmin_WrongPassword_ThenArgumentException()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            var testEmail = "admin@aggierent.com";
            var testPassword = "Verystrongp@ssword";
            var wrongPassword = "Notadminp@assword";

            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = testEmail,
                    HashedPassword = BC.HashPassword(testPassword),
                },
            ];

            mockAdminRepository.Setup(x => x.GetAll()).Returns(admins.AsQueryable());
            var authService = new AuthService(
                mockApplicantRepository.Object,
                mockOwnerRepository.Object,
                mockAdminRepository.Object
            );
            void action() => authService.LoginAdmin(testEmail, wrongPassword);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Wrong password!", ae.Message);
        }
    }
}
