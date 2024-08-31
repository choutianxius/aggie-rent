using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IAuthService
    {
        Applicant LoginApplicant(string email, string password);

        Owner LoginOwner(string email, string password);

        Admin LoginAdmin(string email, string password);

        void RegisterApplicant(
            string email,
            string password,
            string firstName,
            string lastName,
            Gender? gender,
            DateOnly? birthday,
            string? description
        );
    }
}
