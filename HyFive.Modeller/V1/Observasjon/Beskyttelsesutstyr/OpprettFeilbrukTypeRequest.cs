using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class OpprettFeilbrukTypeRequest
    {
        public string Navn { get; set; }
    }
}