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

    // TODO: Test LoginOwner
    // TODO: Test LoginAdmin
}
