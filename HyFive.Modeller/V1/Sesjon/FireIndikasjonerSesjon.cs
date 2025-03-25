using System.Collections.Generic;
using HyFive.Modeller.V1.Observasjon;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Sesjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class FireIndikasjonerSesjon : Sesjon<FireIndikasjonerObservasjon>
    {
    }
}
