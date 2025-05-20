using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HandJewelryType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}