namespace TaskTracker.Models.DTO
{
    public class UserTaskProgressDTO
    {
        public string DisplayName { get; set; } = null!;
        public int CompleteCount { get; set; }
        public int InProgressCount { get; set; }
        public int OverdueCount { get; set; }
        public int TotalCount { get; set; }
        public double PercentComplete { get; set; }
    }
}
