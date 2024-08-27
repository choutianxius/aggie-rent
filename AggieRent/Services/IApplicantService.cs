using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IApplicantService
    {
        Applicant? GetApplicantById(string id);

        IEnumerable<Applicant> GetApplicants(string id);

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
            string? email,
            string? password,
            string? firstName,
            string? lastName,
            Gender? gender,
            DateOnly? birthday,
            string? description
        );

        void DeleteApplicant(string id);
    }
}
