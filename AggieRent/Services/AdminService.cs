using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public class AdminService(IAdminRepository adminRepository) : IAdminService
    {
        private readonly IAdminRepository _adminRepository = adminRepository;

        public Admin? GetAdminById(string Id)
        {
            return null;
        }

        public IEnumerable<Admin> GetAdmins()
        {
            return [];
        }

        public string CreateAdmin(string email, string password)
        {
            return "";
        }

        public void ResetAdminEmail(string id, string email) { }

        public void ResetAdminPassword(string id, string password) { }

        public void DeleteAdmin(string id) { }
    }
}
