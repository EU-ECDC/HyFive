using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreateRegionRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
