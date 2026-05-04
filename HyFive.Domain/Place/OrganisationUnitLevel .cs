using HyFive.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Place
{
    public class OrganisationUnitLevel : AuditableEntity
    {
        public int Id { get; set; }
        public string Level { get; set; }

        public string Description { get; set; }
    }
}
