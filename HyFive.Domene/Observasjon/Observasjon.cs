using System;

namespace HyFive.Domene.Observasjon
{
    public abstract class Observasjon
    {
        public Guid Id { get; set; }
        public DateTime Opprettettidspunkt { get; set; }
        public Rolle Rolle { get; set; }
        public DateTime Registrerttidspunkt { get; set; }
        public string Kommentar { get; set; }
    }
}