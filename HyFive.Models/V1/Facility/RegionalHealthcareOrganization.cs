using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Facility
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class RegionalHealthcareOrganization
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
