using Reinforced.Typings.Attributes;
using System.Collections.Generic;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class FacilityReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public OrganisationUnitType Type { get; set; }
        [TsProperty(ForceNullable = true)]
        public string City { get; set; }
    }
}
