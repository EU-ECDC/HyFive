using System.Collections.Generic;

namespace HyFive.Domene.Observasjon.Beskyttelsesutstyr
{
    public class BeskyttelsesutstyrsettingType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
        public ICollection<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType> BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper { get; set; }
    }
}