using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    public abstract class BaseUser
    {
        [Key]
        [Column("id")]
        public required string Id { get; set; }

        [Column("email")]
        public required string Email { get; set; }

        [Column("hashed_password")]
        public required string HashedPassword { get; set; }
    }
}
