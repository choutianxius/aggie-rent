using AggieRent.Models;

namespace AggieRent.DataAccess
{
    public interface IUserRepository
    {
        User? Get(string userId);

        IQueryable<User> GetAll();

        void Add(User user);

        void Update(User user);

        void Delete(User user);
    }
}
