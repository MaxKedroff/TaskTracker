using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskTracker.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [JsonIgnore]
        public ICollection<Task> Tasks { get; set; }


        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [MaxLength(7)]
        public string Color { get; set; }
    }
}
