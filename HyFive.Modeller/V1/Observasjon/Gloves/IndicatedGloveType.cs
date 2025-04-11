using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Gloves
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class IndicatedGloveType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}