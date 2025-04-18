using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        public bool Permissions { get; set; }

        public UserRole UserRole { get; set; }
    }
}
