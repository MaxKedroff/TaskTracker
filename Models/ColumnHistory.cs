namespace TaskTracker.Models
{
    public class ColumnHistory
    {
        public int ColumnHistoryId { get; set; }

        public int TaskId { get; set; }
        public Task Task { get; set; }

        public int? OldColumnId { get; set; }
        public Column? OldColumn { get; set; }

        public int? NewColumnId { get; set; }
        public Column? NewColumn { get; set; }

        public DateTime ChangeDate { get; set; }
    }
}
