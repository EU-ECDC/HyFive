using System.Collections.Generic;

namespace HyFive.Domene.Observasjon.Beskyttelsesutstyr
{
    public class BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType
    {
        public int BeskyttelsesutstyrTypeId { get; set; }
        public BeskyttelsesutstyrType BeskyttelsesutstyrType { get; set; }
        
        public int BeskyttelsesutstyrsettingTypeId { get; set; }
        public BeskyttelsesutstyrsettingType BeskyttelsesutstyrsettingType { get; set; }
        
        public bool ErDefaultIndikert { get; set; }
    }
}