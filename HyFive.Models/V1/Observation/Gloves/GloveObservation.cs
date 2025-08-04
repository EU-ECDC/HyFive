using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.Gloves
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class GloveObservation : Observation
    {
        [TsProperty(ForceNullable = true)] 
        public List<GloveWithIndicationType> GloveWithIndicationTypes { get; set; }

        [TsProperty(ForceNullable = true)]
        public List<GloveWithoutIndicationType> GloveWithoutIndicationTypes { get; set; }

        public bool GloveUsed { get; set; }

        [TsProperty(ForceNullable = true)]
        public PostGloveHandHygieneType PostGloveHandHygieneType { get; set; }
    }
}