using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    public class CreateDepartmentRequest
    {
        public int FacilityId { get; set; }   // Parent OU
        public string Name { get; set; }
        public int DepartmentTypeId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}
