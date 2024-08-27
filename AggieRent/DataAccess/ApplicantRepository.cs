using AggieRent.Models;
using Microsoft.EntityFrameworkCore;

namespace AggieRent.DataAccess
{
    public class ApplicantRepository(ApplicationDbContext context) : IApplicantRepository
    {
        private readonly ApplicationDbContext _context = context;

        public Applicant? Get(string id)
        {
            return _context.Applicants.FirstOrDefault(a => a.Id == id);
        }

        public Applicant? GetVerbose(string id)
        {
            return _context
                .Applicants.Include(a => a.AppliedApartments)
                .Include(a => a.WishedApartments)
                .Include(a => a.OccupiedApartment)
                .FirstOrDefault(a => a.Id == id);
        }

        public IQueryable<Applicant> GetAll()
        {
            return _context.Applicants;
        }

        public void Add(Applicant a)
        {
            _context.Applicants.Add(a);
            _context.SaveChanges();
        }

        public void Update(Applicant a)
        {
            _context.Applicants.Update(a);
            _context.SaveChanges();
        }

        public void Remove(Applicant a)
        {
            _context.Applicants.Remove(a);
            _context.SaveChanges();
        }
    }
}
