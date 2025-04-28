namespace TaskTracker.Models.DTO
{
    public class WeeklyVelocityDTO
    {

        public DateTime WeekStart { get; set; }


        public int TasksCompleted { get; set; }


        public int DaysInPeriod { get; set; }


        public double Velocity { get; set; }
    }
}

