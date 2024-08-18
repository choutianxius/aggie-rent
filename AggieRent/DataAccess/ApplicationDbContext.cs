using AggieRent.Models;
using Microsoft.EntityFrameworkCore;

namespace AggieRent.DataAccess
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder
                .Entity<User>()
                .Property(e => e.Role)
                .HasConversion(v => v.ToString(), v => Enum.Parse<UserRole>(v));
        }
    }
}
