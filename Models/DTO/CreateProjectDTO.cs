namespace TaskTracker.Models.DTO
{
    public class CreateProjectDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
