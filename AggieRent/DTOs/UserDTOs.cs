using AggieRent.Models;

namespace AggieRent.DTOs
{
    public class UserSummaryDTO
    {
        public string Email { get; set; } = "";

        public UserSummaryDTO() { }

        public UserSummaryDTO(User user)
        {
            Email = user.Email;
        }
    }

    public class UserRegisterDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }

        // Any other fields can be added below in the future
    }

    public class UserLoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
