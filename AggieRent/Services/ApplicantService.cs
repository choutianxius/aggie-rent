using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public class ApplicantService(IApplicantRepository applicantRepository) : IApplicantService
    {
        private readonly IApplicantRepository _applicantRepository = applicantRepository;

        public Applicant? GetApplicantById(string id)
        {
            return _applicantRepository.Get(id);
        }

        public IEnumerable<Applicant> GetApplicants()
        {
            return _applicantRepository.GetAll().ToList();
        }

        public string CreateApplicant(
            string email,
            string password,
            string firstName,
            string lastName,
            Gender? gender,
            DateOnly? birthday,
            string? description
        )
        {
            AuthUtils.ValidateRegistrationCredentials(email, password, _applicantRepository);
            var id = Guid.NewGuid().ToString();
            var applicant = new Applicant()
            {
                Id = id,
                Email = AuthUtils.NormalizeEmail(email),
                HashedPassword = BC.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender ?? Gender.NotSet,
                Birthday = birthday,
                Description = description,
            };
            _applicantRepository.Add(applicant);
            return id;
        }

        public void UpdateApplicant(
            string id,
            string? firstName,
            string? lastName,
            Gender? gender,
            DateOnly? birthday,
            string? description
        )
        {
            var applicant =
                _applicantRepository.Get(id)
                ?? throw new ArgumentException("Applicant ID not found");

            if (firstName != null)
            {
                if (firstName.Equals(""))
                    throw new ArgumentException("First name cannot be empty");
                applicant.FirstName = firstName;
            }

            if (lastName != null)
            {
                if (lastName.Equals(""))
                    throw new ArgumentException("Last name cannot be empty");
                applicant.LastName = lastName;
            }

            if (gender != null)
                applicant.Gender = (Gender)gender!;

            if (birthday != null)
                applicant.Birthday = birthday!;

            if (description != null)
                applicant.Description = description;

            _applicantRepository.Update(applicant);
        }

        public void DeleteApplicant(string id)
        {
            var applicant =
                _applicantRepository.Get(id)
                ?? throw new ArgumentException("User ID doesn't exist");
            _applicantRepository.Remove(applicant);
        }
    }
}
