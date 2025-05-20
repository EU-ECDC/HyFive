using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.Gloves
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class PostGloveHandHygieneType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}