using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        public ICollection<SubTask> SubTasks { get; set; }

        [ForeignKey(nameof(UserRole))]
        public int UserRoleId { get; set; }
        public UserRole UserRole {get; set;}

        public Backlog backlog { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        public DateTime Deadline { get; set; }

        [ForeignKey(nameof(Board))]
        public int BoardId { get; set; }
        public Board Board { get; set; }


        public bool IsEpic { get; set; }
        
        

        

        [ForeignKey(nameof(Comment))]
        public int CommentId { get; set; }
        public ICollection<Comment> Comments { get; set; }

        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public Status StatusRef { get; set; }

        
    }
}
