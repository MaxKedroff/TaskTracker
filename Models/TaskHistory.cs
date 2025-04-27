using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Models
{
    public class TaskHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }
        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; } = null!;

        [Required, MaxLength(30)]
        public string OldStatus { get; set; } = null!;

        [Required, MaxLength(30)]
        public string NewStatus { get; set; } = null!;

        [Required]
        public DateTime ChangeDate { get; set; }
    }

}
