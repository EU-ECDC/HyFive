using HyFive.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Place
{
    public class OrganisationUnitType : AuditableEntity
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
