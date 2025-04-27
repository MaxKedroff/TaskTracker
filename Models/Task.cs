using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TaskTracker.Models.DTO;
using System.Text.Json.Serialization;

namespace TaskTracker.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        public virtual ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();

        [ForeignKey("UserRoleId")]
        [JsonIgnore]
        public UserRole? UserRole { get; set; }
        public int? UserRoleId { get; set; }

        public Backlog? Backlog { get; set; }
        public int? BacklogId { get; set; }  // Для связи с Backlog


        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [ForeignKey("PriorityId")]
        public Priority? Priority { get; set; }
        public int? PriorityId { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        public DateTime Deadline { get; set; }

        [JsonIgnore]
        public Column Column { get; set; }
        public int ColumnId { get; set; }

        public bool IsEpic { get; set; }


        [ForeignKey("StatusId")]
        public Status? Status { get; set; }
        public int? StatusId { get; set; }
    }
}