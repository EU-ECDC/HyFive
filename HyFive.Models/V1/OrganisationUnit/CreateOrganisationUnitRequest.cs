using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    public class CreateOrganisationUnitRequest
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public int OrganisationUnitTypeId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Pseudonym { get; set; }
    }
}
