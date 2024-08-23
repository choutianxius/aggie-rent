using AggieRent.DTOs;
using AggieRent.Models;
using Xunit;

namespace AggieRent.Tests.DTOs
{
    public class UserSummaryDTO_ConstructorShould
    {
        [Theory]
        [MemberData(nameof(DifferentUserClass_SetCorrespondingUserRole_Data))]
        public void Constructor_DifferentValidUserClasses_SetCorrespondingUserRole(BaseUser user)
        {
            var userSummaryDTO = new UserSummaryDTO(user);

            UserRole expectedRole;
            if (user is Applicant)
                expectedRole = UserRole.Applicant;
            else if (user is Owner)
                expectedRole = UserRole.Owner;
            else if (user is Admin)
                expectedRole = UserRole.Admin;
            else
                throw new Exception("Bad test data");
            Assert.Equal(user.Email, userSummaryDTO.Email);
            Assert.Equal(expectedRole, userSummaryDTO.Role);
        }

        public static IEnumerable<object[]> DifferentUserClass_SetCorrespondingUserRole_Data =>
            [
                [
                    new Admin()
                    {
                        Email = "admin@tamu.edu",
                        HashedPassword = BC.HashPassword("strongP@ssw0rd"),
                        Id = Guid.NewGuid().ToString()
                    }
                ],
                [
                    new Owner()
                    {
                        Email = "owner@tamu.edu",
                        HashedPassword = BC.HashPassword("strongP@ssw0rd"),
                        Id = Guid.NewGuid().ToString(),
                        Name = "Awsome Owner"
                    }
                ],
                [
                    new Applicant()
                    {
                        Email = "aggie.gigem@tamu.edu",
                        HashedPassword = BC.HashPassword("strongP@ssw0rd"),
                        Id = Guid.NewGuid().ToString(),
                        FirstName = "Aggie",
                        LastName = "Gigem"
                    }
                ]
            ];

        [Fact]
        public void Constructor_UnknownUserClass_ThenArgumentException()
        {
            var invalidUser = new TestUnknownUserClass()
            {
                Email = "admin@tamu.edu",
                HashedPassword = BC.HashPassword("strongP@ssw0rd"),
                Id = Guid.NewGuid().ToString()
            };

            void action()
            {
                var _ = new UserSummaryDTO(invalidUser);
            }

            var ae = Assert.Throws<ArgumentException>(action);
            Assert.Equal($"Invalid user class {invalidUser.GetType()}", ae.Message);
        }
    }

    class TestUnknownUserClass : BaseUser { }
}
