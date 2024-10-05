using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using Xunit;

namespace AggieRent.Tests.Services
{
    public class OwnerService_GetOwnerByIdShould
    {
        [Fact]
        public void GetOwnerById_ValidId_ThenReturnRelatedUser()
        {
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            List<Owner> owners =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "owner@niceestate.com",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Name = "Nice estate",
                },
            ];
            mockOwnerRepository
                .Setup(x => x.GetVerbose(It.IsAny<string>()))
                .Returns((string id) => owners.FirstOrDefault(a => a.Id == id));
            var ownerService = new OwnerService(mockOwnerRepository.Object);

            var returnedOwner = ownerService.GetOwnerById(owners[0].Id);
            var expectedOwner = new Owner()
            {
                Id = owners[0].Id,
                Email = owners[0].Email,
                HashedPassword = owners[0].HashedPassword,
                Name = owners[0].Name,
            };
            Assert.Equivalent(expectedOwner, returnedOwner);
        }

        [Fact]
        public void GetOwnerById_InvalidId_ThenReturnNull()
        {
            var testId = Guid.NewGuid().ToString();
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            List<Owner> owners =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "owner@niceestate.com",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Name = "Nice estate",
                },
            ];
            mockOwnerRepository
                .Setup(x => x.GetVerbose(It.IsAny<string>()))
                .Returns((string id) => owners.FirstOrDefault(a => a.Id == id));
            var ownerService = new OwnerService(mockOwnerRepository.Object);
            var returnedOwner = ownerService.GetOwnerById(testId);

            Assert.Null(returnedOwner);
        }
    }

    public class OwnerService_GetOwnersShould
    {
        [Fact]
        public void GetOwners_ThenReturnAllOwners()
        {
            var mockOwnerRepository = new Mock<IOwnerRepository>();
            List<Owner> owners =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "owner@niceestate.com",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                    Name = "Nice estate",
                },
            ];
            List<Owner> wrong = [];
            mockOwnerRepository.Setup(x => x.GetAll()).Returns(owners.AsQueryable());
            var ownerService = new OwnerService(mockOwnerRepository.Object);
            var returnedOwners = ownerService.GetOwners();
            Assert.Equivalent(owners, returnedOwners);
        }
    }

    public class OwnerService_CreateOwnerShould
    {
        [Fact]
        public void CreateOwner_ValidCredentials_ThenStoreOwner() { }

        [Fact]
        public void CreateOwner_DuplicateEmail_ThenArgumentException() { }

        [Fact]
        public void CreateOwner_InvalidEmailFormat_ThenArgumentException() { }

        [Fact]
        public void CreateOwner_WeakPassword_ThenArgumentException() { }
    }

    public class OwnerService_DeleteOwnerShould
    {
        [Fact]
        public void DeleteOwner_ValidId_ThenDeleteOwner() { }

        [Fact]
        public void DeleteOwner_InvalidId_ThenArgmentException() { }
    }

    public class OwnerService_UpdateOwnerShould
    {
        [Fact]
        public void UpdateOwner_InvalidId_ThenArgumentException() { }

        [Fact]
        public void UpdateOwner_EmptyName_ThenArgumentException() { }

        [Fact]
        public void UpdateOwner_GoodInput_ThenUpdate() { }
    }

    public class OwnerService_ResetOwnerPasswordShould
    {
        [Fact]
        public void ResetOwnerPassword_InvalidId_ThenArgumentException() { }

        [Fact]
        public void ResetOwnerPassword_InvalidEPassword_ThenArgumentException() { }

        [Fact]
        public void ResetOwnerPassword_GoodInput_ThenUpdateOwnerPassword() { }
    }

    public class OwnerService_ResetOwnerEmailShould
    {
        [Fact]
        public void ResetOwnerEmail_InvalidId_ThenArgumentException() { }

        [Fact]
        public void ResetOwnerEmail_InvalidEmailFormat_ThenArgumentException() { }

        [Fact]
        public void ResetOwnerEmail_DuplicateEmail_ThenArgumentException() { }

        [Fact]
        public void ResetOwnerEmail_GoodInput_ThenUpdateOwnerEmail() { }
    }
}
