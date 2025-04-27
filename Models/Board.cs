using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskTracker.Models
{
    public class Board
    {
        [Key]
        public int BoardId { get; set; }

        public ICollection<Column> Columns { get; set; }




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

        [JsonIgnore]
        public Project Project { get; set; }

        [ForeignKey(nameof(Matrix))]
        public int? MatrixId { get; set; }
        public Matrix? Matrix { get; set; }


        
    }
}
