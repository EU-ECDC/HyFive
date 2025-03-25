using System.Collections.Generic;
using HyFive.Domene.Sesjon;

namespace HyFive.Domene.Observasjon.Hansker
{
    public class HanskeObservasjon : Observasjon
    {
        public ICollection<HanskeMedIndikasjonType> HanskeMedIndikasjonTyper { get; set; }
        public ICollection<HanskeUtenIndikasjonType> HanskeUtenIndikasjonTyper { get; set; }
        public bool BenyttetHanske { get; set; }
        public HandhygieneEtterHanskebrukType HandhygieneEtterHanskebrukType { get; set; }
        public HanskeSesjon HanskeSesjon { get; set; }
    }
}