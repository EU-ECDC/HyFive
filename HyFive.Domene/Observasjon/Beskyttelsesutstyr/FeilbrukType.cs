using System.Collections.Generic;

namespace HyFive.Domene.Observasjon.Beskyttelsesutstyr
{
    public class FeilbrukType
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public BeskyttelsesutstyrType BeskyttelsesutstyrType { get; set; }
        public ICollection<Beskyttelsesutstyr> Beskyttelsesutstyr { get; set; }
    }
}