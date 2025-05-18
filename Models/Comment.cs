using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskTracker.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [ForeignKey(nameof(Task))]
        public int TaskId { get; set; }

        [JsonIgnore]
        public Task Task { get; set; }


        [Required]                     
        public int AuthorId { get; set; }
        [JsonIgnore]
        public User Author { get; set; } = null!;

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        
    }
}
