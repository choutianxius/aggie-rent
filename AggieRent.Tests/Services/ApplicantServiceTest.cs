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
        [Theory]
        [InlineData("aggie1@tamu.edu")]
        [InlineData("AggiE1@tAmu.edu")]
        [InlineData("aggie1@tamu.edu ")]
        [InlineData(" aggie1@tamu.edu")]
        [InlineData("aggie1@tamu.edu\t")]
        [InlineData("aggie1 @tamu.edu")]
        public void CreateApplicant_GoodInput_ThenAddApplicantAndReturnId(string email)
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
            Assert.Equal(AuthUtils.NormalizeEmail(email), createdApplicant.Email);
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
            Assert.Equal("Invalid email format", ae.Message);
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
            Assert.Equal("Email already in use", ae.Message);
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
            Assert.Equal("First name cannot be empty", ae.Message);
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
            Assert.Equal("Last name cannot be empty", ae.Message);
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

    public class ApplicantService_UpdateApplicantShould
    {
        [Fact]
        public void UpdateApplicant_GoodInput_ThenUpdateApplicant()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie1@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie2@tamu.edu",
                    HashedPassword = BC.HashPassword("superStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Deer",
                },
            ];
            mockApplicantRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault(applicant => applicant.Id.Equals(id))
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            applicantService.UpdateApplicant(
                applicants[0].Id,
                "Johny",
                "Doey",
                Gender.Male,
                new DateOnly(2000, 1, 1),
                "Hi I'm Johny Doey"
            );

            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once());
            mockApplicantRepository.Verify((x) => x.Update(applicants[0]), Times.Once);

            Assert.Equal("Johny", applicants[0].FirstName);
            Assert.Equal("Doey", applicants[0].LastName);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.Equal(Gender.Male, applicants[0].Gender);
            Assert.Equal(new DateOnly(2000, 1, 1), applicants[0].Birthday);
            Assert.Equal("Hi I'm Johny Doey", applicants[0].Description);
            Assert.Empty(applicants[0].AppliedApartments);
            Assert.Empty(applicants[0].WishedApartments);
            Assert.Null(applicants[0].OccupiedApartmentId);
            Assert.Null(applicants[0].OccupiedApartment);

            Assert.Equal("John", applicants[1].FirstName);
            Assert.Equal("Deer", applicants[1].LastName);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", applicants[1].HashedPassword));
            Assert.Equal(Gender.NotSet, applicants[1].Gender);
            Assert.Null(applicants[1].Birthday);
            Assert.Null(applicants[1].Description);
            Assert.Empty(applicants[1].AppliedApartments);
            Assert.Empty(applicants[1].WishedApartments);
            Assert.Null(applicants[1].OccupiedApartmentId);
            Assert.Null(applicants[1].OccupiedApartment);
        }

        [Fact]
        public void UpdateApplicant_NonExistentId_ThenArgumentException()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie1@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie2@tamu.edu",
                    HashedPassword = BC.HashPassword("superStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Deer",
                },
            ];
            mockApplicantRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault(applicant => applicant.Id.Equals(id))
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.UpdateApplicant(
                    "abcd123456",
                    "Johny",
                    "Doey",
                    Gender.Male,
                    new DateOnly(2000, 1, 1),
                    "Hi I'm Johny Doey"
                );

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Applicant ID not found", ae.Message);
            mockApplicantRepository.Verify((x) => x.Get("abcd123456"), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("\r")]
        public void UpdateApplicant_EmptyFirstName_ThenArgumentException(string badFirstName)
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
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.UpdateApplicant(
                    applicants[0].Id,
                    badFirstName,
                    null,
                    null,
                    null,
                    null
                );
            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("First name cannot be empty", ae.Message);
            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("\r")]
        public void UpdateApplicant_EmptyLastName_ThenArgumentException(string badLastName)
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
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.UpdateApplicant(
                    applicants[0].Id,
                    null,
                    badLastName,
                    null,
                    null,
                    null
                );
            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Last name cannot be empty", ae.Message);
            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }

        [Fact]
        public void UpdateApplicant_NullFields_ThenNotUpdated()
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            applicantService.UpdateApplicant(applicants[0].Id, null, null, null, null, null);

            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            Assert.Single(applicants);
            Assert.Equal("aggie@tamu.edu", applicants[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.Equal("John", applicants[0].FirstName);
            Assert.Equal("Doe", applicants[0].LastName);
            Assert.Equal(Gender.Female, applicants[0].Gender);
            Assert.Equal(new DateOnly(2000, 1, 1), applicants[0].Birthday);
            Assert.Equal("Hi, I'm John Doe", applicants[0].Description);
            Assert.Empty(applicants[0].AppliedApartments);
            Assert.Empty(applicants[0].WishedApartments);
            Assert.Null(applicants[0].OccupiedApartmentId);
            Assert.Null(applicants[0].OccupiedApartment);
            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(applicants[0]), Times.Once);
        }
    }

    public class ApplicantService_ResetApplicantEmailShould
    {
        [Theory]
        [InlineData("aggie1@tamu.edu")]
        [InlineData("Aggie1@tamu.edu")]
        public void ResetApplicantEmail_GoodInput_ThenUpdateToNormalizedEmail(string updatedEmail)
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository.Setup((x) => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            applicantService.ResetApplicantEmail(applicants[0].Id, updatedEmail);

            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.GetAll(), Times.Once);
            Assert.Single(applicants);
            Assert.Equal(AuthUtils.NormalizeEmail(updatedEmail), applicants[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.Equal("John", applicants[0].FirstName);
            Assert.Equal("Doe", applicants[0].LastName);
            Assert.Equal(Gender.Female, applicants[0].Gender);
            Assert.Equal(new DateOnly(2000, 1, 1), applicants[0].Birthday);
            Assert.Equal("Hi, I'm John Doe", applicants[0].Description);
            Assert.Empty(applicants[0].AppliedApartments);
            Assert.Empty(applicants[0].WishedApartments);
            Assert.Null(applicants[0].OccupiedApartmentId);
            Assert.Null(applicants[0].OccupiedApartment);
            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(applicants[0]), Times.Once);
        }

        [Theory]
        [InlineData("aggie@tamu.edu")]
        [InlineData("Aggie@tAmu.edu")]
        public void ResetApplicantEmail_SameEmail_ThenArgumentException(string updatedEmail)
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository.Setup((x) => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() => applicantService.ResetApplicantEmail(applicants[0].Id, updatedEmail);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email is not modified", ae.Message);
            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }

        [Theory]
        [InlineData("aggie1@tamu.edu")]
        [InlineData("Aggie1@tAmu.edu")]
        public void ResetApplicantEmail_EmailInUse_ThenArgumentException(string updatedEmail)
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie1@tamu.edu",
                    HashedPassword = BC.HashPassword("superStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Deer",
                    Gender = Gender.Male,
                    Birthday = new DateOnly(2000, 1, 2),
                    Description = "Hi, I'm John Deer",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository.Setup((x) => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() => applicantService.ResetApplicantEmail(applicants[0].Id, updatedEmail);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email is already used by another account", ae.Message);

            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.GetAll(), Times.Once);

            Assert.Equal("aggie@tamu.edu", applicants[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.Equal("John", applicants[0].FirstName);
            Assert.Equal("Doe", applicants[0].LastName);
            Assert.Equal(Gender.Female, applicants[0].Gender);
            Assert.Equal(new DateOnly(2000, 1, 1), applicants[0].Birthday);
            Assert.Equal("Hi, I'm John Doe", applicants[0].Description);
            Assert.Empty(applicants[0].AppliedApartments);
            Assert.Empty(applicants[0].WishedApartments);
            Assert.Null(applicants[0].OccupiedApartmentId);
            Assert.Null(applicants[0].OccupiedApartment);

            Assert.Equal("aggie1@tamu.edu", applicants[1].Email);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", applicants[1].HashedPassword));
            Assert.Equal("John", applicants[1].FirstName);
            Assert.Equal("Deer", applicants[1].LastName);
            Assert.Equal(Gender.Male, applicants[1].Gender);
            Assert.Equal(new DateOnly(2000, 1, 2), applicants[1].Birthday);
            Assert.Equal("Hi, I'm John Deer", applicants[1].Description);
            Assert.Empty(applicants[1].AppliedApartments);
            Assert.Empty(applicants[1].WishedApartments);
            Assert.Null(applicants[1].OccupiedApartmentId);
            Assert.Null(applicants[1].OccupiedApartment);

            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.GetAll(), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("aggie@")]
        [InlineData("@tamu.edu")]
        [InlineData("admin@[127.0.0.1")]
        public void ResetApplicantEmail_BadEmailFormat_ThenArgumentException(string badEmail)
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie1@tamu.edu",
                    HashedPassword = BC.HashPassword("superStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Deer",
                    Gender = Gender.Male,
                    Birthday = new DateOnly(2000, 1, 2),
                    Description = "Hi, I'm John Deer",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository.Setup((x) => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() => applicantService.ResetApplicantEmail(applicants[0].Id, badEmail);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Invalid email format", ae.Message);

            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.GetAll(), Times.Never);

            Assert.Equal("aggie@tamu.edu", applicants[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.Equal("John", applicants[0].FirstName);
            Assert.Equal("Doe", applicants[0].LastName);
            Assert.Equal(Gender.Female, applicants[0].Gender);
            Assert.Equal(new DateOnly(2000, 1, 1), applicants[0].Birthday);
            Assert.Equal("Hi, I'm John Doe", applicants[0].Description);
            Assert.Empty(applicants[0].AppliedApartments);
            Assert.Empty(applicants[0].WishedApartments);
            Assert.Null(applicants[0].OccupiedApartmentId);
            Assert.Null(applicants[0].OccupiedApartment);

            Assert.Equal("aggie1@tamu.edu", applicants[1].Email);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", applicants[1].HashedPassword));
            Assert.Equal("John", applicants[1].FirstName);
            Assert.Equal("Deer", applicants[1].LastName);
            Assert.Equal(Gender.Male, applicants[1].Gender);
            Assert.Equal(new DateOnly(2000, 1, 2), applicants[1].Birthday);
            Assert.Equal("Hi, I'm John Deer", applicants[1].Description);
            Assert.Empty(applicants[1].AppliedApartments);
            Assert.Empty(applicants[1].WishedApartments);
            Assert.Null(applicants[1].OccupiedApartmentId);
            Assert.Null(applicants[1].OccupiedApartment);

            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }

        [Fact]
        public void ResetApplicantEmail_NonExistentId_ThenArgumentException()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie1@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie2@tamu.edu",
                    HashedPassword = BC.HashPassword("superStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Deer",
                },
            ];
            mockApplicantRepository
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault(applicant => applicant.Id.Equals(id))
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() => applicantService.ResetApplicantEmail("abcd123456", "aggie3@tamu.edu");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Applicant ID not found", ae.Message);
            mockApplicantRepository.Verify((x) => x.Get("abcd123456"), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }
    }

    public class ApplicantService_ResetApplicantPasswordShould
    {
        [Fact]
        public void ResetApplicantPassword_GoodInput_ThenResetPassword()
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository.Setup((x) => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            applicantService.ResetApplicantPassword(applicants[0].Id, "superStr0ngP@ssw0rd");

            Assert.Single(applicants);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.False(BC.Verify("veryStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.Equal("aggie@tamu.edu", applicants[0].Email);
            Assert.Equal("John", applicants[0].FirstName);
            Assert.Equal("Doe", applicants[0].LastName);
            Assert.Equal(Gender.Female, applicants[0].Gender);
            Assert.Equal(new DateOnly(2000, 1, 1), applicants[0].Birthday);
            Assert.Equal("Hi, I'm John Doe", applicants[0].Description);
            Assert.Empty(applicants[0].AppliedApartments);
            Assert.Empty(applicants[0].WishedApartments);
            Assert.Null(applicants[0].OccupiedApartmentId);
            Assert.Null(applicants[0].OccupiedApartment);
            mockApplicantRepository.Verify((x) => x.Get(applicants[0].Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(applicants[0]), Times.Once);
        }

        [Fact]
        public void ResetApplicantPassword_IdNotExists_ThenArgumentException()
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository.Setup((x) => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.ResetApplicantPassword("abcdefg", "superStr0ngP@ssw0rd");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Applicant ID not found", ae.Message);
            mockApplicantRepository.Verify((x) => x.Get("abcdefg"), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
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
        public void ResetApplicantPassword_InvalidPassword_ThenArgumentException(
            string invalidPassword
        )
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
                    Birthday = new DateOnly(2000, 1, 1),
                    Description = "Hi, I'm John Doe",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository.Setup((x) => x.GetAll()).Returns(applicants.AsQueryable());
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() =>
                applicantService.ResetApplicantPassword(applicants[0].Id, invalidPassword);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal(
                "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number",
                ae.Message
            );
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", applicants[0].HashedPassword));
            mockApplicantRepository.Verify((x) => x.Get(It.IsAny<string>()), Times.Once);
            mockApplicantRepository.Verify((x) => x.Update(It.IsAny<Applicant>()), Times.Never);
        }
    }

    public class ApplicantService_DeleteApplicantShould
    {
        [Fact]
        public void DeleteApplicant_ExistingApplicantId_ThenDelete()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie1@tamu.edu",
                    HashedPassword = BC.HashPassword("verySt0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie2@tamu.edu",
                    HashedPassword = BC.HashPassword("superStr0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Deer",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            mockApplicantRepository
                .Setup((x) => x.Remove(It.IsAny<Applicant>()))
                .Callback(
                    (Applicant applicant) =>
                    {
                        bool removed = applicants.Remove(applicant);
                        if (!removed)
                            throw new ArgumentException("Applicant not found");
                    }
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);
            var applicant1 = applicants[0];
            var applicant2Id = applicants[1].Id;

            applicantService.DeleteApplicant(applicants[0].Id);
            Assert.Single(applicants);
            Assert.Equal(applicant2Id, applicants[0].Id);
            Assert.Equal("aggie2@tamu.edu", applicants[0].Email);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", applicants[0].HashedPassword));
            Assert.Equal("John", applicants[0].FirstName);
            Assert.Equal("Deer", applicants[0].LastName);
            Assert.Equal(Gender.NotSet, applicants[0].Gender);
            Assert.Null(applicants[0].Birthday);
            Assert.Null(applicants[0].Description);
            Assert.Empty(applicants[0].AppliedApartments);
            Assert.Empty(applicants[0].WishedApartments);
            Assert.Null(applicants[0].OccupiedApartmentId);
            Assert.Null(applicants[0].OccupiedApartment);
            mockApplicantRepository.Verify((x) => x.Get(applicant1.Id), Times.Once);
            mockApplicantRepository.Verify((x) => x.Remove(applicant1), Times.Once);
        }

        [Fact]
        public void DeleteApplicant_NonExistentId_ThenArgumentException()
        {
            var mockApplicantRepository = new Mock<IApplicantRepository>();
            List<Applicant> applicants =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie1@tamu.edu",
                    HashedPassword = BC.HashPassword("verySt0ngP@ssw0rd"),
                    FirstName = "John",
                    LastName = "Doe",
                },
            ];
            mockApplicantRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns(
                    (string id) => applicants.FirstOrDefault((applicant) => applicant.Id.Equals(id))
                );
            var applicantService = new ApplicantService(mockApplicantRepository.Object);

            void action() => applicantService.DeleteApplicant("abcd");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Applicant ID not found", ae.Message);
            mockApplicantRepository.Verify((x) => x.Get("abcd"), Times.Once);
            mockApplicantRepository.Verify((x) => x.Remove(It.IsAny<Applicant>()), Times.Never);
        }
    }
}
