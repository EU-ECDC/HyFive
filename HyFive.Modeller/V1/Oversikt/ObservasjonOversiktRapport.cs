using System;
using System.Collections.Generic;
using HyFive.Modeller.V1.Observasjon;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using HyFive.Modeller.V1.Observasjon.Hansker;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Oversikt
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ObservasjonOversiktRapport
    {
        public Guid Id { get; set; }
        public Rolle Rolle { get; set; }
        public string Kommentar { get; set; }
        public DateTime Registrerttidspunkt { get; set; }
        public List<IndikasjonType> Indikasjonstyper { get; set; }
        public Aktivitet Aktivitet { get; set; }
        public List<HandsmykkeType> Handsmykketyper { get; set; }
        public string Beskyttelsesutstyrsetting { get; set; }
        public List<BeskyttelsesutstyrOversiktRapport> Beskyttelsesutstyr { get; set; }
        public HanskeObservasjon HanskeObservasjon { get; set; }
        public BeskyttelsesutstyrObservasjon BeskyttelsesutstyrObservasjon { get; set; }
    }
}
