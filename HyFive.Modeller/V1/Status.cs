using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class Status
    {
        public bool Suksess { get; set; }
        public string Feilmelding { get; set; }
    }
}
