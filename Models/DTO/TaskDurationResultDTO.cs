namespace TaskTracker.Models.DTO
{
    public class TaskDurationResultDTO
    {

        public double AverageDurationDays { get; set; }

        public IEnumerable<TaskDurationDTO> Durations { get; set; } = null!;
    }
}
