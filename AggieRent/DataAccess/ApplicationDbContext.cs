using AggieRent.Models;
using Microsoft.EntityFrameworkCore;

namespace AggieRent.DataAccess
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }
    }
}
