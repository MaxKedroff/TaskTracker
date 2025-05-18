using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string User_name { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Login { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Password { get; set; }


        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<UserRole>? UserRoles { get; set; }

    }
}
