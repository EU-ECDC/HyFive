using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class IndicationType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
    }
}
