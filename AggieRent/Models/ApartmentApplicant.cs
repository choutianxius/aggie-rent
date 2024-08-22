using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    /// <summary>
    /// Join entity for applicants of an apartment
    /// </summary>
    [Table("apartment_applicant")]
    public class ApartmentApplicant
    {
        [Column("apartment_id")]
        public required string ApartmentId { get; set; }

        [Column("applicant_id")]
        public required string ApplicantId { get; set; }
    }
}
