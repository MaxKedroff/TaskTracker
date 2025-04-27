using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class AssignTaskDTO
    {
        [Required]
        public int AssignedUserRoleId { get; set; }

        [Required]
        public int taskId { get; set; }
    }
}
