using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
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
