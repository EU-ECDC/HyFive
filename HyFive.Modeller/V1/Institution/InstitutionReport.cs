using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Modeller.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class InstitutionReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string HERId { get; set; }
        public InstitutionType InstitutionType { get; set; }
        public Region Region { get; set; }
        public Comment Comment { get; set; }
        public HealthcareEnterprise HealthcareEnterprise { get; set; }
    }
}
