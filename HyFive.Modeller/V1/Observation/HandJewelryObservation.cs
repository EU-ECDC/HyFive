using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    [TsInterface( IncludeNamespace = false, AutoI = false)]
    public class HandJewelryObservation : Observation
    {
        [TsProperty(ForceNullable = true)]
        public List<HandJewelryType> HandJewelry { get; set; }
    }
}
