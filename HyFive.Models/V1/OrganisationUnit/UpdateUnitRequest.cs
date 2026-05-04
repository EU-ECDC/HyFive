using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]

    public class UpdateUnitRequest
    {
        public int Id { get; set; }

        public int FacilityId { get; set; }

        public List<int> DepartmentIds { get; set; } 

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
    }
}
