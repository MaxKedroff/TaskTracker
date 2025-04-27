namespace TaskTracker.Models.DTO
{
    public class Priority
    {
        public int PriorityId { get; set; }
        public string Title { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
