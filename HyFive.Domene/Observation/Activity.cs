
namespace HyFive.Domain.Observation
{
    public class Activity
    {
        public int Id { get; set; }

        public int TimeSpent { get; set; }

        public bool TimeRecordingWasDone { get; set; }
        
        public bool? GloveUsed { get; set; }

        public ActivityType ActivityType { get; set; }
    }
}
