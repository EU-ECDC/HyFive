using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.Gloves
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class GloveWithIndicationType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}