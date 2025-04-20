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

        public Permission Permissions { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public static class SystemRoles
    {
        public const int Admin = 1;
        public const int Employee = 2;
    }

    public enum Permission : long
    {
        None = 0,
        CreateTask = 1 << 0,
        EditTask = 1 << 1,
        DeleteTask = 1 << 2,
        AssignTask = 1 << 3,
        ManageMembers = 1 << 4,
    }
}

