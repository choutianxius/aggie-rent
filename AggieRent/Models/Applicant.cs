using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    [Table("applicant")]
    public class Applicant : BaseUser
    {
        [Column("first_name")]
        public required string FirstName { get; set; }

        [Column("last_name")]
        public required string LastName { get; set; }

        [Column("gender")]
        public Gender Gender { get; set; }

        [Column("birthday")]
        public DateOnly? Birthday { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        public ICollection<Apartment> AppliedApartments { get; set; } = [];

        public ICollection<Apartment> WishList { get; set; } = [];

        [Column("occupied_apartment_id")]
        public string? OccupiedApartmentId { get; set; }

        public Apartment? OccupiedApartment { get; set; }
    }

    public enum Gender
    {
        NotSet,
        Female,
        Male,
        NonBinary
    }
}
