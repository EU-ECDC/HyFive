using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.Gloves
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class GloveObservation : Observation
    {
        [TsProperty(ForceNullable = true)] 
        public List<IndicatedGloveType> IndicatedGloveTypes { get; set; }

        [TsProperty(ForceNullable = true)]
        public List<GeneralPurposeGloveType> GeneralPurposeGloveTypes { get; set; }

        public bool GloveUsed { get; set; }

        [TsProperty(ForceNullable = true)]
        public PostGloveHandHygieneType PostGloveHandHygieneType { get; set; }
    }
}