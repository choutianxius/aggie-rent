using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public void Register(string email, string password)
        {
            // Lazy execute
            var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
                throw new ArgumentException("Email already in use!");
            User user =
                new()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = email,
                    HashedPassword = BC.HashPassword(password)
                };
            _userRepository.Add(user);
        }

        public void Login(string email, string password) { }

        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetAll().ToList();
        }
    }
}
