using System.Net.Mail;
using System.Text.RegularExpressions;
using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public partial class AuthService(
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
            ValidateRegistrationCredentials(email, password, _applicantRepository);
            var applicant = new Applicant()
            {
                Id = Guid.NewGuid().ToString(),
                Email = NormalizeEmail(email),
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
                repository.GetAll().FirstOrDefault(u => u.Email.Equals(NormalizeEmail(email)))
                ?? throw new ArgumentException("Email not registered!");
            if (!BC.Verify(password, user.HashedPassword))
                throw new ArgumentException("Wrong password!");
            return user;
        }

        private static void ValidateRegistrationCredentials<TUser>(
            string email,
            string password,
            IBaseRepository<TUser> repository
        )
            where TUser : BaseUser
        {
            string normalizedEmail = NormalizeEmail(email);
            if (!ValidateEmail(normalizedEmail))
                throw new ArgumentException("Invalid email format!");

            // Lazy execute
            var existingUser = repository.GetAll().FirstOrDefault(u => u.Email == normalizedEmail);
            if (existingUser != null)
                throw new ArgumentException("Email already in use!");

            if (!ValidatePassword(password))
                throw new ArgumentException(
                    "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number"
                );
        }

        private static string NormalizeEmail(string email)
        {
            return email.ToLower();
        }

        private static bool ValidateEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
        private static partial Regex PasswordRegex();

        private static readonly Regex passwordRegex = PasswordRegex();

        /// <summary>
        /// Verify the format of a password during registration
        /// </summary>
        private static bool ValidatePassword(string password)
        {
            return passwordRegex.IsMatch(password);
        }
    }
}
