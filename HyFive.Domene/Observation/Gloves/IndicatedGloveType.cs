using System.Collections.Generic;

namespace HyFive.Domain.Observation.Gloves
{
    public class IndicatedGloveType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ICollection<GloveObservation> Observations { get; set; }
    }
}