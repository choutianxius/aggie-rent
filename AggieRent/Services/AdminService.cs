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

        public void ResetAdminEmail(string id, string email)
        {
            var normalizedEmail = AuthUtils.NormalizeEmail(email);
            if (!AuthUtils.ValidateEmail(normalizedEmail))
                throw new ArgumentException("Invalid email format");
            var admin =
                _adminRepository.Get(id) ?? throw new ArgumentException("Admin ID not found");
            if (admin.Email.Equals(normalizedEmail))
                throw new ArgumentException("Email is not modified");
            if (
                _adminRepository
                    .GetAll()
                    .FirstOrDefault((admin) => admin.Email.Equals(normalizedEmail)) != null
            )
                throw new ArgumentException("Email already in use");
            admin.Email = normalizedEmail;
            _adminRepository.Update(admin);
        }

        public void ResetAdminPassword(string id, string password) { }

        public void DeleteAdmin(string id) { }
    }
}
