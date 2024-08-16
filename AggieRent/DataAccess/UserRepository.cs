using AggieRent.Models;

namespace AggieRent.DataAccess
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        private readonly ApplicationDbContext _context = context;

        public User? Get(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public IQueryable<User> GetAll()
        {
            return _context.Users;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
