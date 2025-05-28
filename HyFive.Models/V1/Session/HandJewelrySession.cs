using HyFive.Models.V1.Observation;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HandJewelrySession : Session<HandJewelryObservation>
    {
    }
}
