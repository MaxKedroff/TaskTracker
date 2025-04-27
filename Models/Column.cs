using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskTracker.Models
{
    public class Column
    {
        [Key]
        public int ColumnID { get; set; }

        [JsonIgnore]
        public Board Board { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public ICollection<Defect> Defects { get; set; }


        public string Title { get; set; }

        public string Color { get; set; }

        
    }
}
