using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    [TsEnum(IncludeNamespace = false)]
    public enum IndicationTypeId
    {
        PrePatient=1,
        AsepticProcedures=2,
        BodilyFluid=3,
        PostPatient=4
    }
}
