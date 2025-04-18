using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Models
{

    public class Matrix
    {
        [Key]
        public int MatrixId { get; set; }

        public Board Board { get; set; }


        public int Sector { get; set; }
        public string TaskInfo { get; set; }

        public int Epic_id { get; set; }

    }
}
