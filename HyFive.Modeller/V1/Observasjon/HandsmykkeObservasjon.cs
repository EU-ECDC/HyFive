using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface( IncludeNamespace = false, AutoI = false)]
    public class HandsmykkeObservasjon : Observasjon
    {
        [TsProperty(ForceNullable = true)]
        public List<HandsmykkeType> Handsmykker { get; set; }
    }
}
