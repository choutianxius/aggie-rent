using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IUserService
    {
        void Register(string email, string password);

        User Login(string email, string password);

        IEnumerable<User> GetUsers();

        User GetUserById(string userId);

        void UpdateUser(User updatedUser);

        void DeleteUser(string userId);
    }
}
