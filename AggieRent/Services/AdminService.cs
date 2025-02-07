using AggieRent.DataAccess;
using AggieRent.Models;

namespace AggieRent.Services
{
    public class AdminService(IAdminRepository adminRepository) : IAdminService
    {
        private readonly IAdminRepository _adminRepository = adminRepository;

        public Admin? GetAdminById(string id)
        {
            return _adminRepository.GetVerbose(id);
        }

        public IEnumerable<Admin> GetAdmins()
        {
            return _adminRepository.GetAll().ToList();
        }

        public string CreateAdmin(string email, string password)
        {
            var normalizedEmail = AuthUtils.NormalizeEmail(email);
            AuthUtils.ValidateRegistrationCredentials(normalizedEmail, password, _adminRepository);
            var id = Guid.NewGuid().ToString();
            _adminRepository.Add(
                new()
                {
                    Id = id,
                    Email = normalizedEmail,
                    HashedPassword = BC.HashPassword(password),
                }
            );
            return id;
        }

        public void ResetAdminEmail(string id, string email) { }

        public void ResetAdminPassword(string id, string password) { }

        public void DeleteAdmin(string id) { }
    }
}
