using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    [Table("Apartment")]
    public class Apartment
    {
        [Key]
        [Column("apt_id")]
        public required string AptId { get; set; }

        [Column("poster_id")]
        public required string PosterId { get; set; }

        [Column("creation_time")]
        public required DateTime CreationTime { get; set; }

        [Column("last_edit_time")]
        public required DateTime LastEditTime { get; set; }

        [Column("apt_name")]
        public required string AptName { get; set; }

        [Column("size")]
        public required double Size { get; set; }

        [Column("type")]
        public required string Type { get; set; }

        [Column("address_line1")]
        public required string AddressLine1 { get; set; }

        [Column("address_line2")]
        public required string AddressLine2 { get; set; }

        [Column("desc")]
        public string Description { get; set; } = "";

        [Column("available_date")]
        public required DateTime AvaiableDate { get; set; }
    }
}
