using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IUserService
    {
        void Register(string email, string password);

        void Login(string email, string password);

        IEnumerable<User> GetUsers();
    }
}
