using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;
using Moq;
using Xunit;

namespace AggieRent.Tests.Services
{
    public class AdminsService_GetAdminByIdShould
    {
        [Fact]
        public void GetAdminById_GoodId_ThenReturnAdmin()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin1@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin2@tamu.edu",
                    HashedPassword = BC.HashPassword("superStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.GetVerbose(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            var adminService = new AdminService(mockAdminRepository.Object);

            var admin = adminService.GetAdminById(admins[0].Id);

            Assert.NotNull(admin);
            Assert.Equal(admins[0].Id, admin.Id);
            Assert.Equal("admin1@tamu.edu", admin.Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admin.HashedPassword));
            Assert.Equal(2, admins.Count);
            mockAdminRepository.Verify((x) => x.GetVerbose(admin.Id), Times.Once);
            mockAdminRepository.Verify((x) => x.Get(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetAdminById_NonExistentId_ThenReturnNull()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin1@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.GetVerbose(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            var adminService = new AdminService(mockAdminRepository.Object);

            var admin = adminService.GetAdminById("abcde12345");

            Assert.Null(admin);
            Assert.Single(admins);
            mockAdminRepository.Verify((x) => x.GetVerbose("abcde12345"), Times.Once);
            mockAdminRepository.Verify((x) => x.Get(It.IsAny<string>()), Times.Never);
        }
    }

    public class AdminService_GetAdminsShould
    {
        [Fact]
        public void GetAdmins_ThenReturnAdmins()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin1@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository.Setup((x) => x.GetAll()).Returns(admins.AsQueryable());
            var adminService = new AdminService(mockAdminRepository.Object);

            var returnedAdmins = adminService.GetAdmins();

            Assert.Equivalent(admins, returnedAdmins);
            mockAdminRepository.Verify((x) => x.GetAll(), Times.Once);
        }
    }

    public class AdminService_CreateAdminShould
    {
        [Theory]
        [InlineData("admin2@tamu.edu")]
        [InlineData(" ADMIN2@tamu.edu")]
        [InlineData("aDmin2@TAMU.EDU")]
        [InlineData("aDmin2@TAMU.edu ")]
        [InlineData("admin2 @tamu.edu")]
        public void CreateAdmin_GoodInput_ThenReturnCreatedId(string email)
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin1@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            mockAdminRepository
                .Setup((x) => x.Add(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        if (admins.FirstOrDefault((admin_) => admin_.Id.Equals(admin.Id)) != null)
                            throw new ArgumentException("Duplicate ID");
                        admins.Add(admin);
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            var admin1Id = admins[0].Id;
            var createdId = adminService.CreateAdmin(email, "superStr0ngP@ssw0rd");
            Assert.Equal(2, admins.Count);
            var admin1 = admins.FirstOrDefault((a) => a.Id.Equals(admin1Id));
            Assert.NotNull(admin1);
            Assert.Equal("admin1@tamu.edu", admin1.Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admin1.HashedPassword));
            var admin2 = admins.FirstOrDefault((a) => a.Id.Equals(createdId));
            Assert.NotNull(admin2);
            Assert.Equal("admin2@tamu.edu", admin2.Email);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", admin2.HashedPassword));
            mockAdminRepository.Verify((x) => x.Add(admin2), Times.Once);
        }
    }
}
