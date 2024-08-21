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
        public required string Email { get; set; }

        [Column("hashed_password")]
        public required string HashedPassword { get; set; }

        [Column("role")]
        public required UserRole Role { get; set; }
    }

    public enum UserRole
    {
        User,
        Admin
    }
}
