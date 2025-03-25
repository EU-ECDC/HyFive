using HyFive.Modeller.V1.Observasjon.Hansker;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Sesjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HanskeSesjon : Sesjon<HanskeObservasjon>
    {
    }
}
