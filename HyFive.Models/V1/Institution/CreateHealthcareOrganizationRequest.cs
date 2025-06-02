using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreateHealthcareOrganizationRequest
    {
        public string Name { get; set; }
        public int RegionalHealthcareOrganizationId { get; set; }
    }
}
