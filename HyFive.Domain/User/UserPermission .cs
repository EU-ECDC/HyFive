using HyFive.Domain.Common;
using HyFive.Domain.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.User
{
    public  class UserPermission : AuditableEntity
    {
        public int Id { get; set; } 

        public string PermissionLevel { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? OrganisationUnitId { get; set; }
        public OrganisationUnit OrganisationUnit { get; set; } = null!;
    }
}
