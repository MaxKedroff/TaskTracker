namespace TaskTracker.Models.DTO
{
    public class TaskDurationDTO
    {
        public int TaskId { get; set; }
        public int DurationDays { get; set; }

        public bool IsAnomaly { get; set; }
    }
}
