namespace TaskTracker.Models.DTO
{
    public class WeeklyFlowDTO
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }

        public List<ColumnCountDTO> Columns { get; set; } = new();

        public int TotalTasks { get; set; }
        public double PercentDone { get; set; }   // (готово / всего) * 100
    }
}
