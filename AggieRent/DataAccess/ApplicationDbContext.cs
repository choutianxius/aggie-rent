using AggieRent.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AggieRent.DataAccess
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}
