using AggieRent.Models;
using Microsoft.EntityFrameworkCore;

namespace AggieRent.DataAccess
{
    public class OwnerRepository(ApplicationDbContext context) : IOwnerRepository
    {
        private readonly ApplicationDbContext _context = context;

        public Owner? Get(string id)
        {
            return _context.Owners.FirstOrDefault(o => o.Id == id);
        }

        public Owner? GetVerbose(string id)
        {
            return _context.Owners.Include(o => o.OwnedApartments).FirstOrDefault(o => o.Id == id);
        }

        public IQueryable<Owner> GetAll()
        {
            return _context.Owners;
        }

        public void Add(Owner owner)
        {
            _context.Owners.Add(owner);
            _context.SaveChanges();
        }

        public void Update(Owner owner)
        {
            _context.Owners.Update(owner);
            _context.SaveChanges();
        }

        public void Remove(Owner owner)
        {
            _context.Owners.Remove(owner);
            _context.SaveChanges();
        }
    }
}
