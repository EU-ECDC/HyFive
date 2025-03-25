using System;
using HyFive.Domene.Bruker;
using HyFive.Domene.Sted;

namespace HyFive.Domene.Sesjon
{

    public class Sesjon
    {
        public DateTime Opprettettidspunkt { get; set; }
        public DateTime Starttidspunkt { get; set; }
        public Guid Id { get; set; }
        public Avdeling Avdeling { get; set; }
        public Observator Observator { get; set; }
        public string Kommentar { get; set; }
        public string Discriminator { get; set; }
        public OverforingstatusType Overforingstatus { get; set; }
    }
}
