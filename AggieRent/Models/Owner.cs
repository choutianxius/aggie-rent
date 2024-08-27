using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    [Table("owner")]
    public class Owner : BaseUser
    {
        [Column("name")]
        public required string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        public ICollection<Apartment> OwnedApartments { get; set; } = [];
    }
}
