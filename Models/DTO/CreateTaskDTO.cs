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
        public string currentColumn { get; set; }


        [Required]
        public int statusId { get; set; }

        [Required]
        public int priorityId { get; set; }

        public int? AssignedUserRoleId { get; set; }

        [Required]
        public int columnId { get; set; }

    }
}
