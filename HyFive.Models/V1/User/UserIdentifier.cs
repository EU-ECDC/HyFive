using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.User
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class UserIdentifier
    {
        public int Id { get; set; }

        // If you keep it as a string in the DB (not ideal, but mirror it)
        public string IdentifierType { get; set; }

        public string Value { get; set; }

        public int UserId { get; set; }

        public int UserIdentifierTypeId { get; set; }

        // Optional: include if your API returns the type object too
        [TsProperty(ForceNullable = true)]
        public UserIdentifierType UserIdentifierType { get; set; }
    }
}
