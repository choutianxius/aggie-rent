using System.Text.RegularExpressions;
using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public partial class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly Regex emailRegex = EmailRegex();

        public void Register(string email, string password)
        {
            string normalizedEmail = email.ToLower();
            if (!emailRegex.IsMatch(normalizedEmail))
                throw new ArgumentException("Invalid email format!");

            // Lazy execute
            var existingUser = _userRepository
                .GetAll()
                .FirstOrDefault(u => u.Email == normalizedEmail);
            if (existingUser != null)
                throw new ArgumentException("Email already in use!");
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

        [GeneratedRegex(
            "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"
        )]
        private static partial Regex EmailRegex();
    }
}
