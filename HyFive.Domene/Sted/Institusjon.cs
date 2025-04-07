using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyFive.Domene.Sted
{
    public class Institusjon
    {
        public Institusjon()
        {
            Opprettettidspunkt = DateTime.UtcNow;
            Brukere = new List<Bruker.Bruker>();
            PredefinerteKommmentarer = new List<PredefinertKommentar>();
        }
        public int Id { get; set; }
        public string HERId { get; set; }
        public DateTime Opprettettidspunkt { get; set; }
        public string Navn { get; set; }
        public ICollection<Klinikk> Klinikker { get; set; }
        public ICollection<Avdeling> Avdelinger { get; set; }
        public string Forkortelse { get; set; }
        [Column("Bruker")]
        public ICollection<Bruker.Bruker> Brukere { get; set; }
        public ICollection<PredefinertKommentar> PredefinerteKommmentarer { get; set; }
        public InstitusjonType Institusjontype { get; set; }
        public Region Region { get; set; }
        public Helseforetak Helseforetak { get; set; }
        public Kommune Kommune { get; set; }
    }
}
