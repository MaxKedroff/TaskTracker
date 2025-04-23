using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class CreateTaskDTO
    {
        [Required, MaxLength(100)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public int BoardId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string? Status { get; set; }

        [Required]
        public string? Priority { get; set; }

        public int? AssignedUserRoleId { get; set; }

    }
}
