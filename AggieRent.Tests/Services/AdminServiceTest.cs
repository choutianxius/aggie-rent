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

        [Theory]
        [InlineData("aggie@tamu.edu")]
        [InlineData("aggie @ tamu.edu")]
        [InlineData("AGGIE@TAMU.EDU")]
        [InlineData("Aggie@tamu.edu")]
        [InlineData("aggie@TAMU.EDU")]
        [InlineData("aggie@tamu.edu ")]
        [InlineData(" aggie@tamu.edu")]
        [InlineData(" aggie@tamu.edu\n")]
        public void CreateAdmin_DuplicateEmail_ThenArgumentException(string email)
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository.Setup((x) => x.GetAll()).Returns(admins.AsQueryable());
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

            void action() => adminService.CreateAdmin(email, "superStr0ngP@ssw0rd");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email already in use", ae.Message);
            Assert.Single(admins);
            Assert.Equal("aggie@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.GetAll(), Times.Once);
            mockAdminRepository.Verify((x) => x.Add(It.IsAny<Admin>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("aggie@")]
        [InlineData("@tamu.edu")]
        [InlineData("admin@[127.0.0.1")]
        public void CreateAdmin_InvalidEmail_ThenArgumentException(string email)
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.CreateAdmin(email, "veryStr0ngP@ssw0rd");

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
        public void CreateAdmin_InvalidPassword_ThenArgumentException(string password)
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

            void action() => adminService.CreateAdmin("admin2@tamu.edu", password);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal(
                "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number",
                ae.Message
            );
            Assert.Single(admins);
            Assert.Equal("admin1@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Add(It.IsAny<Admin>()), Times.Never);
        }
    }

    public class AdminService_ResetAdminEmailShould
    {
        [Theory]
        [InlineData("admin2@tamu.edu")]
        [InlineData(" ADMIN2@tamu.edu")]
        [InlineData("aDmin2@TAMU.EDU")]
        [InlineData("aDmin2@TAMU.edu ")]
        [InlineData("admin2 @tamu.edu")]
        public void ResetAdminEmail_GoodEmail_ThenReset(string email)
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
            mockAdminRepository.Setup((x) => x.GetAll()).Returns(admins.AsQueryable());
            mockAdminRepository
                .Setup((x) => x.Update(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        var existingAdmin =
                            admins.FirstOrDefault((admin1) => admin1.Id.Equals(admin.Id))
                            ?? throw new ArgumentException("Nonexistent ID");
                        existingAdmin.Email = admin.Email;
                        existingAdmin.HashedPassword = admin.HashedPassword;
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            adminService.ResetAdminEmail(admins[0].Id, email);

            Assert.Single(admins);
            Assert.Equal("admin2@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get(admins[0].Id), Times.Once);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("aggie@")]
        [InlineData("@tamu.edu")]
        [InlineData("admin@[127.0.0.1")]
        public void ResetAdminEmail_InvalidEmail_ThenArgumentException(string email)
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
            mockAdminRepository.Setup((x) => x.GetAll()).Returns(admins.AsQueryable());
            mockAdminRepository
                .Setup((x) => x.Update(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        var existingAdmin =
                            admins.FirstOrDefault((admin1) => admin1.Id.Equals(admin.Id))
                            ?? throw new ArgumentException("Nonexistent ID");
                        existingAdmin.Email = admin.Email;
                        existingAdmin.HashedPassword = admin.HashedPassword;
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.ResetAdminEmail(admins[0].Id, email);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Invalid email format", ae.Message);
            Assert.Single(admins);
            Assert.Equal("admin1@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get(admins[0].Id), Times.Never);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Never);
        }

        [Theory]
        [InlineData("aggie@tamu.edu")]
        [InlineData("aggie @ tamu.edu")]
        [InlineData("AGGIE@TAMU.EDU")]
        [InlineData("Aggie@tamu.edu")]
        [InlineData("aggie@TAMU.EDU")]
        [InlineData("aggie@tamu.edu ")]
        [InlineData(" aggie@tamu.edu")]
        [InlineData(" aggie@tamu.edu\n")]
        public void ResetAdminEmail_IdenticalEmail_ThenArgumentException(string email)
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            mockAdminRepository.Setup((x) => x.GetAll()).Returns(admins.AsQueryable());
            mockAdminRepository
                .Setup((x) => x.Update(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        var existingAdmin =
                            admins.FirstOrDefault((admin1) => admin1.Id.Equals(admin.Id))
                            ?? throw new ArgumentException("Nonexistent ID");
                        existingAdmin.Email = admin.Email;
                        existingAdmin.HashedPassword = admin.HashedPassword;
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.ResetAdminEmail(admins[0].Id, email);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email is not modified", ae.Message);
            Assert.Single(admins);
            Assert.Equal("aggie@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get(admins[0].Id), Times.Once);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Never);
        }

        [Theory]
        [InlineData("admin2@tamu.edu")]
        [InlineData(" ADMIN2@tamu.edu")]
        [InlineData("aDmin2@TAMU.EDU")]
        [InlineData("aDmin2@TAMU.edu ")]
        [InlineData("admin2 @tamu.edu")]
        public void ResetAdminEmail_EmailAlreadyInUse_ThenArgumentException(string email)
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
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            mockAdminRepository.Setup((x) => x.GetAll()).Returns(admins.AsQueryable());
            mockAdminRepository
                .Setup((x) => x.Update(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        var existingAdmin =
                            admins.FirstOrDefault((admin1) => admin1.Id.Equals(admin.Id))
                            ?? throw new ArgumentException("Nonexistent ID");
                        existingAdmin.Email = admin.Email;
                        existingAdmin.HashedPassword = admin.HashedPassword;
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.ResetAdminEmail(admins[0].Id, email);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Email already in use", ae.Message);
            Assert.Equal(2, admins.Count);
            Assert.Equal("admin1@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            Assert.Equal("admin2@tamu.edu", admins[1].Email);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", admins[1].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get(admins[0].Id), Times.Once);
            mockAdminRepository.Verify((x) => x.GetAll(), Times.Once);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Never);
        }

        [Fact]
        public void ResetAdminEmail_UnknownId_ThenArgumentException()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "aggie@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));

            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.ResetAdminEmail("abcd1234", "aggie1@tamu.edu");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Admin ID not found", ae.Message);
            Assert.Single(admins);
            Assert.Equal("aggie@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get("abcd1234"), Times.Once);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Never);
        }
    }

    public class AdminService_ResetAdminPasswordShould
    {
        [Fact]
        public void ResetAdminPassword_GoodInput_ThenReset()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            mockAdminRepository
                .Setup((x) => x.Update(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        var existingAdmin =
                            admins.FirstOrDefault((admin1) => admin1.Id.Equals(admin.Id))
                            ?? throw new ArgumentException("Nonexistent ID");
                        existingAdmin.Email = admin.Email;
                        existingAdmin.HashedPassword = admin.HashedPassword;
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            adminService.ResetAdminPassword(admins[0].Id, "superStr0ngP@ssw0rd");

            Assert.Single(admins);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", admins[0].HashedPassword));
            Assert.False(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get(admins[0].Id), Times.Once);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Once);
        }

        [Fact]
        public void ResetAdminPassword_UnknownAdminId_ThenArgumentException()
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            mockAdminRepository
                .Setup((x) => x.Update(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        var existingAdmin =
                            admins.FirstOrDefault((admin1) => admin1.Id.Equals(admin.Id))
                            ?? throw new ArgumentException("Nonexistent ID");
                        existingAdmin.Email = admin.Email;
                        existingAdmin.HashedPassword = admin.HashedPassword;
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.ResetAdminPassword("abcd1234", "superStr0ngP@ssw0rd");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Admin ID not found", ae.Message);
            Assert.Single(admins);
            Assert.False(BC.Verify("superStr0ngP@ssw0rd", admins[0].HashedPassword));
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get("abcd1234"), Times.Once);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Never);
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
        public void ResetAdminPassword_InvalidNewPassword_ThenArgumentException(string password)
        {
            var mockAdminRepository = new Mock<IAdminRepository>();
            List<Admin> admins =
            [
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin@tamu.edu",
                    HashedPassword = BC.HashPassword("veryStr0ngP@ssw0rd"),
                },
            ];
            mockAdminRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            mockAdminRepository
                .Setup((x) => x.Update(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        var existingAdmin =
                            admins.FirstOrDefault((admin1) => admin1.Id.Equals(admin.Id))
                            ?? throw new ArgumentException("Nonexistent ID");
                        existingAdmin.Email = admin.Email;
                        existingAdmin.HashedPassword = admin.HashedPassword;
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.ResetAdminPassword(admins[0].Id, password);

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal(
                "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number",
                ae.Message
            );
            Assert.Single(admins);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get(admins[0].Id), Times.Once);
            mockAdminRepository.Verify((x) => x.Update(It.IsAny<Admin>()), Times.Never);
        }
    }

    public class AdminService_DeleteAdminShould
    {
        [Fact]
        public void DeleteAdmin_ExistentId_ThenDelete()
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
            var admin1Id = admins[0].Id;
            var admin1 = admins[0];
            var admin2Id = admins[1].Id;
            mockAdminRepository
                .Setup((x) => x.Get(It.IsAny<string>()))
                .Returns((string id) => admins.FirstOrDefault((admin) => admin.Id.Equals(id)));
            mockAdminRepository
                .Setup((x) => x.Remove(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        if (!admins.Contains(admin))
                            throw new ArgumentException("Non-existent entity");
                        admins.Remove(admin);
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            adminService.DeleteAdmin(admins[0].Id);

            Assert.Single(admins);
            var admin2 = admins.FirstOrDefault((admin) => admin.Id.Equals(admin2Id));
            Assert.NotNull(admin2);
            Assert.Equal("admin2@tamu.edu", admin2.Email);
            Assert.True(BC.Verify("superStr0ngP@ssw0rd", admin2.HashedPassword));
            mockAdminRepository.Verify((x) => x.Get(admin1Id), Times.Once);
            mockAdminRepository.Verify((x) => x.Get(It.IsAny<string>()), Times.Once);
            mockAdminRepository.Verify((x) => x.Remove(admin1), Times.Once);
            mockAdminRepository.Verify((x) => x.Remove(It.IsAny<Admin>()), Times.Once);
        }

        [Fact]
        public void DeleteAdmin_NonExistentId_ThenArgumentException()
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
                .Setup((x) => x.Remove(It.IsAny<Admin>()))
                .Callback(
                    (Admin admin) =>
                    {
                        if (!admins.Contains(admin))
                            throw new ArgumentException("Non-existent entity");
                        admins.Remove(admin);
                    }
                );
            var adminService = new AdminService(mockAdminRepository.Object);

            void action() => adminService.DeleteAdmin("abcd1234");

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Admin ID not found", ae.Message);
            Assert.Single(admins);
            Assert.Equal("admin1@tamu.edu", admins[0].Email);
            Assert.True(BC.Verify("veryStr0ngP@ssw0rd", admins[0].HashedPassword));
            mockAdminRepository.Verify((x) => x.Get("abcd1234"), Times.Once);
            mockAdminRepository.Verify((x) => x.Get(It.IsAny<string>()), Times.Once);
            mockAdminRepository.Verify((x) => x.Remove(It.IsAny<Admin>()), Times.Never);
        }
    }
}
