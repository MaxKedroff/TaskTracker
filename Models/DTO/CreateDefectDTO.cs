using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models.DTO
{
    public class CreateDefectDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }


        public int priorityId { get; set; }


        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int boardId { get; set; }

        public string currentColumn { get; set; }

        public int projectId { get; set; }

    }
}
