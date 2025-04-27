namespace TaskTracker.Models
{
    public class Severity
    {
        public int SeverityId { get; set; }
        public string Title { get; set; }
        public ICollection<Defect> Defects { get; set; }
    }
}
