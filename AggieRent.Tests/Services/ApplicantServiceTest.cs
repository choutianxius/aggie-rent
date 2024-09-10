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
}
