using System;

namespace HyFive.Domene.Bruker
{
    public class ForesporselOmBrukertilgang
    {
        public int Id { get; set; }
        public int? InstitusjonId { get; set; }
        public string BrukerFornavn { get; set; }
        public string BrukerEtternavn { get; set; }
        public string IdentPseudonym { get; set; }
        public string HPRNummer { get; set; }
        public ForesporselOmBrukertilgangStatus Status { get; set; }
        public DateTime Opprettettidspunkt { get; set; }
        public DateTime? BehandletTidspunkt { get; set; }
        public int? BehandletAvBrukerId { get; set; }
        public string BehandletAvBrukernavn { get; set; }
    }

    public enum ForesporselOmBrukertilgangStatus
    {
        Registrert,
        Godkjent,
        Avvist
    }
}
