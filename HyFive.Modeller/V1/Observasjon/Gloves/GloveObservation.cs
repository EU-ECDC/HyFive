using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Gloves
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class GloveObservation : Observasjon
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