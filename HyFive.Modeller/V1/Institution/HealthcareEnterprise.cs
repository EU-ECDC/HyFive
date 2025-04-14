using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HealthcareEnterprise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RegionaltHealthcareEnterpriseId { get; set; }
        public RegionalInstitution RegionalHealthcareEnterprise { get; set; }
    }
}
