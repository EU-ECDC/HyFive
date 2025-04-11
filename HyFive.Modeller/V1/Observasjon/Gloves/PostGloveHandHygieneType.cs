using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon.Gloves
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class PostGloveHandHygieneType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}