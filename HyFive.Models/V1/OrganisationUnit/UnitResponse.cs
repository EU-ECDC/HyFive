using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    public class UnitResponse
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string Name { get; set; }
        public List<OrganisationUnit> Departments { get; set; }
    }
}
