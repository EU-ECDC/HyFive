using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
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
