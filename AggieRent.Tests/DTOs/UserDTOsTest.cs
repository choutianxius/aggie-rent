using AggieRent.DTOs;
using AggieRent.Models;
using Xunit;

namespace AggieRent.Tests.DTOs
{
    public class UserSummaryDTO_ConstructorShould
    {
        [Fact]
        public void Constructor_ThenSetEmail()
        {
            // Arrange
            var user = new User()
            {
                Email = "aggie@tamu.edu",
                HashedPassword = BC.HashPassword("123456"),
                UserId = Guid.NewGuid().ToString()
            };

            // Act
            var userSummaryDTO = new UserSummaryDTO(user);

            // Assert
            Assert.Equal(user.Email, userSummaryDTO.Email);
        }
    }
}
