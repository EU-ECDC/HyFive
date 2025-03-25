using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Hansker
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HanskeObservasjon : Observasjon
    {
        [TsProperty(ForceNullable = true)] 
        public List<HanskeMedIndikasjonType> HanskeMedIndikasjonTyper { get; set; }

        [TsProperty(ForceNullable = true)]
        public List<HanskeUtenIndikasjonType> HanskeUtenIndikasjonTyper { get; set; }

        public bool BenyttetHanske { get; set; }

        [TsProperty(ForceNullable = true)]
        public HandhygieneEtterHanskebrukType HandhygieneEtterHanskebrukType { get; set; }
    }
}