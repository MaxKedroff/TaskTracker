using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class EditTaskDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }

        public int? AssignedUserRoleId { get; set; }
    }
}
