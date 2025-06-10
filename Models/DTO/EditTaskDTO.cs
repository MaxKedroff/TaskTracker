using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class EditTaskDTO
    {
        public int taskId { get; set; }

        public string? Title { get; set; }

        public string? currentColumn { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }

        public int? statusId { get; set; }

        public int? priorityId { get; set; }

        public int? AssignedUserRoleId { get; set; }

        public string? Color { get; set; }
    }
}
