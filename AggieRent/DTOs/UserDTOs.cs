using AggieRent.Models;

namespace AggieRent.DTOs
{
    public class UserSummaryDTO
    {
        public string Email { get; set; } = "";
        public UserRole Role { get; set; }

        public UserSummaryDTO() { }

        public UserSummaryDTO(BaseUser user)
        {
            Email = user.Email;
            if (user is Applicant)
                Role = UserRole.Applicant;
            else if (user is Owner)
                Role = UserRole.Owner;
            else if (user is Admin)
                Role = UserRole.Admin;
            else
                throw new ArgumentException($"Invalid user class {user.GetType()}");
        }
    }

    public enum UserRole
    {
        Applicant,
        Owner,
        Admin
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
