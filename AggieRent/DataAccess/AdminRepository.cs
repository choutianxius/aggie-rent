using AggieRent.Models;

namespace AggieRent.DataAccess
{
    public class AdminRepository(ApplicationDbContext context) : IAdminRepository
    {
        private readonly ApplicationDbContext _context = context;

        public Admin? Get(string id)
        {
            return _context.Admins.FirstOrDefault(a => a.Id == id);
        }

        public Admin? GetVerbose(string id)
        {
            return _context.Admins.FirstOrDefault(a => a.Id == id);
        }

        public IQueryable<Admin> GetAll()
        {
            return _context.Admins;
        }

        public void Add(Admin admin)
        {
            _context.Admins.Add(admin);
            _context.SaveChanges();
        }

        public void Update(Admin admin)
        {
            _context.Admins.Update(admin);
            _context.SaveChanges();
        }

        public void Remove(Admin admin)
        {
            _context.Admins.Remove(admin);
            _context.SaveChanges();
        }
    }
}
