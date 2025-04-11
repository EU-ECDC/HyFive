using HyFive.Modeller.V1.Observasjon.Gloves;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Sesjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HanskeSesjon : Sesjon<GloveObservation>
    {
    }
}
