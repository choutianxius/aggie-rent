using AggieRent.Models;

namespace AggieRent.DTOs
{
    public record class LoginDTO(string Email, string Password);

    public record class ApplicantRegistrationDTO(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        Gender Gender = Gender.NotSet,
        DateOnly? Birthday = null,
        string Description = ""
    );
}
