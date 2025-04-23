using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class CreateBoardDTO
    {
        [Required, MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int ProjectId { get; set; }
    }
}
