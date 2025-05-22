using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class CreateColumnDTO
    {


        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Color { get; set; } = "#ffffff"; // Значение по умолчанию
    }
}
