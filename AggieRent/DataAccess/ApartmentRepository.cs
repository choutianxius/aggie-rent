using AggieRent.Models;
using Microsoft.EntityFrameworkCore;

namespace AggieRent.DataAccess
{
    public class ApartmentRepository(ApplicationDbContext context) : IApartmentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public Apartment? Get(string id)
        {
            return _context.Apartments.FirstOrDefault(apt => apt.Id == id);
        }

        public Apartment? GetVerbose(string id)
        {
            return _context
                .Apartments.Include(apt => apt.Applicants)
                .FirstOrDefault(apt => apt.Id == id);
        }

        public IQueryable<Apartment> GetAll()
        {
            return _context.Apartments;
        }

        public void Add(Apartment apartment)
        {
            _context.Apartments.Add(apartment);
            _context.SaveChanges();
        }

        public void Update(Apartment apartment)
        {
            _context.Apartments.Update(apartment);
            _context.SaveChanges();
        }

        public void Remove(Apartment apartment)
        {
            _context.Apartments.Remove(apartment);
            _context.SaveChanges();
        }
    }
}
