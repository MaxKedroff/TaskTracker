using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Models
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User user { get; set; }

        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [ForeignKey(nameof(Task))]
        public int TaskId { get; set; }
        public ICollection<Task> Tasks { get; set; }

        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
