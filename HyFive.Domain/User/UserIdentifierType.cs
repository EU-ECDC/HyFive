using HyFive.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.User
{
    public  class UserIdentifierType : AuditableEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!; // e.g. "HPR"
        public string Name { get; set; } = null!; // e.g. "HPR Number"
    }
}
