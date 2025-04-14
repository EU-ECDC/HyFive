using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Institution
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string HERId { get; set; }
        public List<Department> Departments { get; set; }
        public InstitutionType InstitutionType { get; set; }
        public Region Region { get; set; }
        public Comment Comment { get; set; }
        public bool HasObservations { get; set; }
        public HealthcareEnterprise HealthcareProvider { get; set; }
    }
}
