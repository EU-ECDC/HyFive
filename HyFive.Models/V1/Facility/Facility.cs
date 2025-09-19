using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Facility
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string HERId { get; set; }
        public List<Department> Departments { get; set; }
        public FacilityType FacilityType { get; set; }
        public Region Region { get; set; }
        public Municipality Municipality { get; set; }
        public bool HasObservations { get; set; }
        public HealthcareOrganization HealthcareOrganization { get; set; }
    }
}
