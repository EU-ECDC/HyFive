using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyFive.Domene.Observasjon
{
    [Table("IndikasjonType")]
    public class IndikasjonType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
        public string Nummer { get; set; }
        public ICollection<FireIndikasjonerObservasjon> Observasjoner { get; set; }
    }
}
