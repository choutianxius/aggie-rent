using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IApplicantService
    {
        Applicant? GetApplicantById(string id);

        IEnumerable<Applicant> GetApplicants();

        string CreateApplicant(
            string email,
            string password,
            string firstName,
            string lastName,
            Gender? gender,
            DateOnly? birthday,
            string? description
        );

        void UpdateApplicant(
            string id,
            string? firstName,
            string? lastName,
            Gender? gender,
            DateOnly? birthday,
            string? description
        );

        void ResetApplicantEmail(string id, string newEmail);

        void ResetApplicantPassword(string id, string newPassword);

        void DeleteApplicant(string id);
    }
}
