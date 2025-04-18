using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Board
    {
        [Key]
        public int BoardId { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public ICollection<Defect> Defects { get; set; }



        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey(nameof(Matrix))]
        public int MatrixId { get; set; }
        public Matrix Matrix { get; set; }


        
    }
}
