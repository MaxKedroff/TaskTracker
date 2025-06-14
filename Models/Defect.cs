using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskTracker.Models.DTO;

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


        [ForeignKey("PriorityId")]
        public Priority? Priority { get; set; }
        public int? PriorityId { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(Column))]
        public int ColumnId { get; set; }
        [JsonIgnore]
        public Column Column { get; set; }

        public bool IsDone { get; set; }

    }


}
