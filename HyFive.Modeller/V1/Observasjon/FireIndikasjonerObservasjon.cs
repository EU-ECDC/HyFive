using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface( IncludeNamespace = false, AutoI = false)]
    public class FireIndikasjonerObservasjon : Observasjon
    {
        [TsProperty(ForceNullable = true)]
        public List<IndikasjonType> Indikasjonstyper { get; set; }

        [TsProperty(ForceNullable = true)]
        public Aktivitet Aktivitet { get; set; }
    }
}
