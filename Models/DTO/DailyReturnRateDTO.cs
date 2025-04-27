namespace TaskTracker.Models.DTO
{
    public class DailyReturnRateDTO
    {
        public DateTime Date { get; set; }
        public int CompletedTasks { get; set; }
        public int ReturnedTasks { get; set; }
        /// <summary>
        /// Return rate as a percentage (0–100).
        /// </summary>
        public double ReturnRate { get; set; }
    }
}
