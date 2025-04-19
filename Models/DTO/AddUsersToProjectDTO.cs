namespace TaskTracker.Models.DTO
{
    public class AddUsersToProjectDTO
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }   
        public int RoleId { get; set; } = SystemRoles.Employee;
    }
}
