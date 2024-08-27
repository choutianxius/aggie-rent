using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public class AuthService(
        IApplicantRepository applicantRepository,
        IOwnerRepository ownerRepository,
        IAdminRepository adminRepository
    ) : IAuthService
    {
        private readonly IApplicantRepository _applicantRepository = applicantRepository;
        private readonly IOwnerRepository _ownerRepository = ownerRepository;
        private readonly IAdminRepository _adminRepository = adminRepository;

        public Applicant LoginApplicant(string email, string password)
        {
            return LoginUser(email, password, _applicantRepository);
        }

        public Owner LoginOwner(string email, string password)
        {
            return LoginUser(email, password, _ownerRepository);
        }

        public Admin LoginAdmin(string email, string password)
        {
            return LoginUser(email, password, _adminRepository);
        }

        public void RegisterApplicant(
            string email,
            string password,
            string firstName,
            string lastName,
            Gender? gender = Gender.NotSet,
            DateOnly? birthday = null,
            string? description = null
        )
        {
            AuthUtils.ValidateRegistrationCredentials(email, password, _applicantRepository);
            var applicant = new Applicant()
            {
                Id = Guid.NewGuid().ToString(),
                Email = AuthUtils.NormalizeEmail(email),
                HashedPassword = BC.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender ?? Gender.NotSet,
                Birthday = birthday,
                Description = description,
            };
            _applicantRepository.Add(applicant);
        }

        private static TUser LoginUser<TUser>(
            string email,
            string password,
            IBaseRepository<TUser> repository
        )
            where TUser : BaseUser
        {
            var user =
                repository
                    .GetAll()
                    .FirstOrDefault(u => u.Email.Equals(AuthUtils.NormalizeEmail(email)))
                ?? throw new ArgumentException("Email not registered!");
            if (!BC.Verify(password, user.HashedPassword))
                throw new ArgumentException("Wrong password!");
            return user;
        }
    }
}
