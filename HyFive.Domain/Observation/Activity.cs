
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HyFive.Domain.Observation
{
    public class Activity
    {
        public int Id { get; set; }

        public int SecondsUsed { get; set; }

        public bool TimingWasPerformed { get; set; }
        
        public bool? GlovesUsed { get; set; }

        public ActivityType ActivityType { get; set; }
    }
}
