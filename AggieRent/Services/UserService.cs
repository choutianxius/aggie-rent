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
            string normalizedEmail = email.ToLower();
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

        public void Login(string email, string password) { }

        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetAll().ToList();
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

        private static bool ValidatePassword(string password)
        {
            return passwordRegex.IsMatch(password);
        }
    }
}
