using System.Collections.Generic;
using HyFive.Domain.Session;

namespace HyFive.Domain.Observation.Gloves
{
    public class GloveObservation : Observation
    {
        public ICollection<GloveWithIndicationType> GloveWithIndicationTypes { get; set; }
        public ICollection<GloveWithoutIndicationType> GloveWithoutIndicationTypes { get; set; }
        public bool GlovesUsed { get; set; }
        public HandHygieneAfterGloveUseType PostGloveHandHygieneType { get; set; }
        public GloveSession GloveSession { get; set; }
    }
}