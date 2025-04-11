using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreateHealthEnterpriseRequest
    {
        public string Name { get; set; }
        public int RegionaltHealthcareEnterprise { get; set; }
    }
}
