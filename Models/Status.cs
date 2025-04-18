using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        public Task Task { get; set; }


        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [MaxLength(7)]
        public string Color { get; set; }
    }
}
