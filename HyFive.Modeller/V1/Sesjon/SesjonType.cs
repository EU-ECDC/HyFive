using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Sesjon
{

    [TsEnum(IncludeNamespace = false)]
    public enum SesjonType {
        FireIndikasjoner = 1,
        InnUt = 2,
        Handsmykker = 3,
        Hansker = 4,
        Beskyttelsesutstyr = 5,
        IkkeValgt = 6
    }
}
