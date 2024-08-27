using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IAdminService
    {
        Admin? GetAdminById(string id);

        IEnumerable<Admin> GetAdmins(string id);

        string CreateAdmin(string email, string password);

        void UpdateAdmin(string id, string? email, string? password);

        void DeleteAdmin(string id);
    }
}
