using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;
using Moq;
using Xunit;

namespace AggieRent.Tests.Services
{
    public class ApplicantService_GetApplicantByIdShould
    {
        [Fact]
        public void GetApplicantById_ValidId_ThenReturnRelatedUser()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = Gender.Female,
                },
            ];
            mockApplicantRepository
                .Setup(x => x.GetVerbose(It.IsAny<string>()))
                .Returns((string id) => applicants.FirstOrDefault(a => a.Id == id));
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            var returnedApplicant = applicantService.GetApplicantById(applicants[0].Id);
            var expectedApplicant = new Applicant()
            {
                Id = applicants[0].Id,
                Email = applicants[0].Email,
                HashedPassword = applicants[0].HashedPassword,
                FirstName = applicants[0].FirstName,
                LastName = applicants[0].LastName,
                Gender = applicants[0].Gender,
                Birthday = applicants[0].Birthday,
                OccupiedApartmentId = applicants[0].OccupiedApartmentId,
            };
            Assert.Equivalent(expectedApplicant, returnedApplicant);
            mockApplicantRepository.Verify(x => x.GetVerbose(applicants[0].Id), Times.Once());
        }

        [Fact]
        public void GetApplicantById_UnknownId_ThenReturnNull()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants = [];
            mockApplicantRepository
                .Setup(x => x.GetVerbose(It.IsAny<string>()))
                .Returns((string id) => applicants.FirstOrDefault(a => a.Id == id));
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            var idToFind = Guid.NewGuid().ToString();
            var returnedApplicant = applicantService.GetApplicantById(idToFind);

            Assert.Null(returnedApplicant);
            mockApplicantRepository.Verify(x => x.GetVerbose(idToFind), Times.Once());
        }
    }

    public class ApplicantService_GetApplicantsShould
    {
        [Fact]
        public void GetApplicants_ThenOk()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
            ];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            var returnedApplicants = applicantService.GetApplicants();
            var expectedApplicants = new List<Applicant>(applicants);
            Assert.Equivalent(expectedApplicants, returnedApplicants);
            mockApplicantRepository.Verify(x => x.GetAll(), Times.Once());
        }
    }

    public class ApplicantService_CreateApplicantShould
    {
        [Fact]
        public void CreateApplicant_GoodInput_ThenAddApplicantAndReturnId()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
            ];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            mockApplicantRepository
                .Setup(x => x.Add(It.IsAny<Applicant>()))
                .Callback(
                    (Applicant a) =>
                    {
                        if (applicants.FirstOrDefault(a1 => a1.Id == a.Id) != null)
                            throw new Exception("Duplicate ID");
                        applicants.Add(a);
                    }
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            var email = "aggie1@tamu.edu";
            var password = "superStr0ngp@ssw0rd";
            var firstName = "John";
            var lastName = "Deer";
            var gender = Gender.Female;
            var birthday = new DateOnly(2000, 1, 1);
            var description = "Hi I'm John Deer";
            var id = applicantService.CreateApplicant(
                email,
                password,
                firstName,
                lastName,
                gender,
                birthday,
                description
            );

            Assert.Equal(2, applicants.Count);
            var createdApplicant = applicants.FirstOrDefault(a => a.Id == id);
            Assert.NotNull(createdApplicant);
            Assert.Equal(email, createdApplicant.Email);
            Assert.True(BC.Verify(password, createdApplicant.HashedPassword));
            Assert.Equal(firstName, createdApplicant.FirstName);
            Assert.Equal(lastName, createdApplicant.LastName);
            Assert.Equal(gender, createdApplicant.Gender);
            Assert.Equivalent(new DateOnly(2000, 1, 1), createdApplicant.Birthday);
            Assert.Equal(description, createdApplicant.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("aggie@")]
        [InlineData("@tamu.edu")]
        [InlineData("admin@[127.0.0.1")]
        public void CreateApplicant_InvalidEmail_ThenArgumentException(string badEmail)
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.CreateApplicant(
                    badEmail,
                    "superStr0ngP@ssw0rd",
                    "John",
                    "Doe",
                    null,
                    null,
                    null
                );

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Invalid email format!", ae.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("aB1_")]
        [InlineData("abcdefgh")]
        [InlineData("a1b2c3d4")]
        [InlineData("a1b2c3d_")]
        [InlineData("superstrongp@ssw0rd")]
        [InlineData("SUPERSTRONGP@SSW0RD")]
        [InlineData("SuperStrongPassw0rd")]
        [InlineData("SuperStrongP@ssword")]
        [InlineData("Emoj1S_n0T_All0w3d🥲")]
        public void CreateApplicant_InvalidPassword_ThenArgumentException(string badPassword)
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.CreateApplicant(
                    "aggie@tamu.edu",
                    badPassword,
                    "John",
                    "Doe",
                    null,
                    null,
                    null
                );

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal(
                "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number",
                ae.Message
            );
        }

        [Theory]
        [InlineData("Aggie@tamu.edu")]
        [InlineData("aggIe@TAMU.EDU")]
        public void CreateApplicant_DuplicateEmail_ThenArgumentException(string badEmail)
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
            ];
            mockApplicantRepository.Setup(x => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.CreateApplicant(
                    badEmail,
                    "superStr0ngP@ssw0rd",
                    "John",
                    "Deer",
                    null,
                    null,
                    null
                );

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email already in use!", ae.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        public void CreateApplicant_EmptyFirstName_ThenArgumentException(string emptyFirstName)
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.CreateApplicant(
                    "aggie@tamu.edu",
                    "superStr0ngP@ssw0rd",
                    emptyFirstName,
                    "Doe",
                    null,
                    null,
                    null
                );

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("First name cannot be empty!", ae.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        public void CreateApplicant_EmptyLastName_ThenArgumentException(string emptyLastName)
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.CreateApplicant(
                    "aggie@tamu.edu",
                    "superStr0ngP@ssw0rd",
                    "John",
                    emptyLastName,
                    null,
                    null,
                    null
                );

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Last name cannot be empty!", ae.Message);
        }

        [Fact]
        public void CreateApplicant_NullGender_ThenDefaultValue()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants = [];
            mockApplicantRepository
                .Setup(x => x.Add(It.IsAny<Applicant>()))
                .Callback(
                    (Applicant a) =>
                    {
                        if (applicants.FirstOrDefault(a1 => a1.Id == a.Id) != null)
                            throw new Exception("Duplicate ID");
                        applicants.Add(a);
                    }
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            applicantService.CreateApplicant(
                "aggie@tamu.edu",
                "veryStr0ngP@ssw0Rd",
                "John",
                "Doe",
                null,
                null,
                null
            );

            Assert.Single(applicants);
            Assert.Equal(Gender.NotSet, applicants[0].Gender);
        }
    }
}
