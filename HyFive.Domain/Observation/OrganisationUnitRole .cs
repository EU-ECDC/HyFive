using HyFive.Domain.Common;
using HyFive.Domain.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Observation
{
    public class OrganisationUnitRole : AuditableEntity
    {
        public int Id { get; set; }

        public int OrganisationUnitId { get; set; }
        public OrganisationUnit OrganisationUnit { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
