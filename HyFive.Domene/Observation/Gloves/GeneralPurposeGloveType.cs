using System.Collections.Generic;

namespace HyFive.Domain.Observation.Gloves
{
    public class GeneralPurposeGloveType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ICollection<GloveObservation> Observations { get; set; }
    }
}