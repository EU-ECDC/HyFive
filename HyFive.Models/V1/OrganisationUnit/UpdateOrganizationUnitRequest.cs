using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    public class UpdateOrganizationUnitRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public int OrganisationUnitTypeId { get; set; }

        public Address Address { get; set; }
    }
}
