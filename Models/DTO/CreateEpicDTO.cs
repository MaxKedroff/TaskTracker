namespace TaskTracker.Models.DTO
{
    public class CreateEpicDTO
    {
        public int BoardId { get; set; }
        public string CurrentColumn { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Deadline { get; set; }

        // при желании можно сделать необязательными
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public int? AssignedUserRoleId { get; set; }
    }
}
