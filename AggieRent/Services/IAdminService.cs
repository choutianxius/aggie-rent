using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IAdminService
    {
        Admin? GetAdminById(string id);

        IEnumerable<Admin> GetAdmins();

        string CreateAdmin(string email, string password);

        void ResetAdminEmail(string id, string email);

        void ResetAdminPassword(string id, string password);

        void DeleteAdmin(string id);
    }
}
