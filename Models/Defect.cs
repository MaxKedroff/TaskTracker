using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Defect
    {
        [Key]
        public int DefectId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public string Severity { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(Column))]
        public int ColumnId { get; set; }
        public Column Column { get; set; }
    }

    
}
