using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    /// <summary>
    /// Join entity for apartments on an applicant's wishlist
    /// </summary>
    [Table("wished_apartment")]
    public class WishedApartment
    {
        [Column("applicant_id")]
        public required string ApplicantId { get; set; }

        [Column("apartment_id")]
        public required string ApartmentId { get; set; }
    }
}
