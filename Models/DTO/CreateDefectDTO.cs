using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class CreateDefectDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public string Severity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int boardId { get; set; }

        public int ColumnId { get; set; }

    }
}
