using System.Collections.Generic;

namespace HyFive.Domene.Sesjon
{
    public class OverforingstatusType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
        public ICollection<Sesjon> Sesjoner { get; set; }
    }
}