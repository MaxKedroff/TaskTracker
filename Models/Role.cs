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

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public static class SystemRoles
    {
        public const int Admin = 1;
        public const int Employee = 2;
    }
}
