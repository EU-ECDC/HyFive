using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class HealthcareOrganization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RegionalHealthcareOrganizationId { get; set; }
        public RegionalHealthcareOrganization RegionalHealthcareOrganization { get; set; }
    }
}
