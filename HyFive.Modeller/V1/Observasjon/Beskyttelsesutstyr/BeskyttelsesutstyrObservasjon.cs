using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class BeskyttelsesutstyrObservasjon : Observasjon
    {
        public List<Beskyttelsesutstyr> Beskyttelsesutstyrliste { get; set; }
        public BeskyttelsesutstyrsettingType Settingtype { get; set; }
    }
}