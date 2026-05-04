using HyFive.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.User
{
    public class UserIdentifier : AuditableEntity
    {
        public int Id { get; set; }

        public int UserIdentifierTypeId { get; set; }
        public UserIdentifierType UserIdentifierType { get; set; }

        public string Value { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
