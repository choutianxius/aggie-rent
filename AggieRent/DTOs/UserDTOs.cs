using AggieRent.Models;

namespace AggieRent.DTOs
{
    public class UserSummaryDTO(User user)
    {
        public string Email { get; set; } = user.Email;
        public UserRole Role { get; set; } = user.Role;
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
