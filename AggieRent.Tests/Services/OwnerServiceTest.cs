using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;
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
                .Setup(x => x.Get(It.IsAny<string>()))
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
    }

    public class OwnerService_GetOwnersShould { }

    public class OwnerService_CreateOwnerShould { }

    public class OwnerService_DeleteOwnerShould { }

    public class OwnerService_UpdateOwnerShould { }

    public class OwnerService_ResetOwnerPasswordShould { }

    public class OwnerService_ResetOwnerEmailShould { }
}
