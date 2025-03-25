using System;
using System.Collections.Generic;
using HyFive.Modeller.V1.Institusjon;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Oversikt
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class SesjonOversiktRapport
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string ObservatorNavn { get; set; }
        public Avdeling Avdeling { get; set; }
        public DateTime Starttidspunkt { get; set; }
        public DateTime Opprettettidspunkt { get; set; }
        public string Kommentar { get; set; }
        public OverforingstatusType Overforingstatus { get; set; }
        public List<ObservasjonOversiktRapport> Observasjoner { get; set; }
        public bool ErValgt { get; set; }
    }
}
