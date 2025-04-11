using System.Collections.Generic;
using HyFive.Domain.Session;

namespace HyFive.Domain.Observation.Gloves
{
    public class GloveObservation : Observation
    {
        public ICollection<IndicatedGloveType> IndicatedGloveTypes { get; set; }
        public ICollection<GeneralPurposeGloveType> GeneralPurposeGloveTypes { get; set; }
        public bool GloveUsed { get; set; }
        public PostGloveHandHygiene PostGloveHandHygieneType { get; set; }
        public GloveSession GloveSession { get; set; }
    }
}