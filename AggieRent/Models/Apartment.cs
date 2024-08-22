using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    [Table("apartment")]
    public class Apartment
    {
        [Key]
        [Column("id")]
        public required string Id { get; set; }

        [Column("owner_id")]
        public required string OwnerId { get; set; }

        public required Owner Owner { get; set; }

        [Column("last_edited_at")]
        public required DateTime LastEditedAt { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("description")]
        public string Description { get; set; } = "";

        [Column("size")]
        public required double Size { get; set; }

        [Column("bedrooms")]
        public required int Bedrooms { get; set; }

        [Column("bathrooms")]
        public required int Bathrooms { get; set; }

        [Column("apt_count")]
        public required int AptCount { get; set; }

        [Column("photo_urls")]
        public ICollection<string> PhotoUrls { get; set; } = [];

        [Column("available_from")]
        public required DateOnly AvailableFrom { get; set; }

        [Column("available_to")]
        public required DateOnly AvailableTo { get; set; }

        [Column("address_line1")]
        public required string AddressLine1 { get; set; }

        [Column("address_line2")]
        public required string AddressLine2 { get; set; }

        [Column("city")]
        public required string City { get; set; }

        [Column("state")]
        public required USState State { get; set; }

        [Column("zip_code")]
        public required string ZipCode { get; set; }

        [Column("occupied")]
        public required bool Occupied { get; set; }

        public ICollection<Applicant> Applicants { get; set; } = [];
    }

    public enum USState
    {
        Alabama,
        Alaska,
        Arizona,
        Arkansas,
        California,
        Colorado,
        Connecticut,
        Delaware,
        Florida,
        Georgia,
        Hawaii,
        Idaho,
        Illinois,
        Indiana,
        Iowa,
        Kansas,
        Kentucky,
        Louisiana,
        Maine,
        Maryland,
        Massachusetts,
        Michigan,
        Minnesota,
        Mississippi,
        Missouri,
        Montana,
        Nebraska,
        Nevada,
        NewHampshire,
        NewJersey,
        NewMexico,
        NewYork,
        NorthCarolina,
        NorthDakota,
        Ohio,
        Oklahoma,
        Oregon,
        Pennsylvania,
        RhodeIsland,
        SouthCarolina,
        SouthDakota,
        Tennessee,
        Texas,
        Utah,
        Vermont,
        Virginia,
        Washington,
        WestVirginia,
        Wisconsin,
        Wyoming
    }
}
