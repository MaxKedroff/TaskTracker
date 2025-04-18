using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Backlog
    {
        [Key]
        public int BacklogId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Status { get; set; }

        public string User { get; set; }  

        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateEnd { get; set; }

        [ForeignKey(nameof(Task))]
        public int TaskId { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
