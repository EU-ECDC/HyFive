using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    [TsInterface( IncludeNamespace = false, AutoI = false)]
    public class FourIndicatorsObservation : Observation
    {
        [TsProperty(ForceNullable = true)]
        public List<IndicationType> IndicationTypes { get; set; }

        [TsProperty(ForceNullable = true)]
        public Activity Activity { get; set; }
    }
}
