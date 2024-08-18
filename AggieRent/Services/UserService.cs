using System.Net.Mail;
using System.Text.RegularExpressions;
using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public partial class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public void Register(string email, string password)
        {
            string normalizedEmail = NormalizeEmail(email);
            if (!ValidateEmail(normalizedEmail))
                throw new ArgumentException("Invalid email format!");

            // Lazy execute
            var existingUser = _userRepository
                .GetAll()
                .FirstOrDefault(u => u.Email == normalizedEmail);
            if (existingUser != null)
                throw new ArgumentException("Email already in use!");

            if (!ValidatePassword(password))
                throw new ArgumentException(
                    "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number"
                );

            User user =
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = normalizedEmail,
                    HashedPassword = BC.HashPassword(password)
                };
            _userRepository.Add(user);
        }

        public User Login(string email, string password)
        {
            var user =
                _userRepository.GetAll().FirstOrDefault(u => u.Email.Equals(NormalizeEmail(email)))
                ?? throw new ArgumentException("Email not registered!");
            if (!BC.Verify(password, user.HashedPassword))
                throw new ArgumentException("Wrong password!");
            return user;
        }

        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetAll().ToList();
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
