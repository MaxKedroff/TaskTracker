using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Models
{
    public class Project
    {

        [Key]
        public int ProjectId { get; set; }

        public ICollection<Board> Boards { get; set; }


        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }


    }
}
