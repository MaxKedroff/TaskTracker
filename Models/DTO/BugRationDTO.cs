namespace TaskTracker.Models.DTO
{
    public enum PeriodKind
    {
        Week,
        Month
    }

    
    public class BugRatioDTO
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public int BugCount { get; set; }
        public int NewTaskCount { get; set; }

        public double Ratio { get; set; }
    }

    
    public class AskBugRatioDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PeriodKind Period { get; set; } = PeriodKind.Week;
    }
}
