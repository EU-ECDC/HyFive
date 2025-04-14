using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Activity
    {
        public ActivityType ActivityType { get; set; }
        
        [TsProperty(ForceNullable = true)]
        public int TimeSpent { get; set; }

        public bool TimeRecordingWasDone { get; set; }
        
        [TsProperty(ForceNullable = true)]
        public bool? GloveUsed { get; set; }
    }
}
