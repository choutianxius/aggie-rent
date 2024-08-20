using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AggieRent.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public required string UserId { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("hashed_password")]
        public string? HashedPassword { get; set; }

        [Column("role")]
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        User,
        Admin
    }
}
