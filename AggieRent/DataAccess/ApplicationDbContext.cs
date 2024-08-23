using AggieRent.Models;
using Microsoft.EntityFrameworkCore;

namespace AggieRent.DataAccess
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public required DbSet<Applicant> Users { get; set; }
        public required DbSet<Owner> Owners { get; set; }
        public required DbSet<Admin> Admins { get; set; }
        public required DbSet<Apartment> Apartments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // DB enum types
            modelBuilder.HasPostgresEnum<UsState>();
            modelBuilder.HasPostgresEnum<Gender>();

            // indexes
            modelBuilder.Entity<Applicant>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Owner>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Admin>().HasIndex(u => u.Email).IsUnique();

            // relationships
            modelBuilder
                .Entity<Apartment>()
                .HasOne(apartment => apartment.Owner)
                .WithMany(owner => owner.OwnedApartments)
                .HasForeignKey(apartment => apartment.OwnerId);
            modelBuilder
                .Entity<Apartment>()
                .HasMany(apartment => apartment.Applicants)
                .WithMany(applicant => applicant.AppliedApartments)
                .UsingEntity<ApartmentApplicant>();

            modelBuilder
                .Entity<Applicant>()
                .HasMany(applicant => applicant.WishedApartments)
                .WithMany() // unidirectional many-to-many
                .UsingEntity<WishedApartment>();
            modelBuilder
                .Entity<Applicant>()
                .HasOne(applicant => applicant.OccupiedApartment)
                .WithMany() // unidirectional one-to-many
                .HasForeignKey(applicant => applicant.OccupiedApartmentId);
        }
    }
}
