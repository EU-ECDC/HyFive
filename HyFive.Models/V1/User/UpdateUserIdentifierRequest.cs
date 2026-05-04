using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.User
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class UpdateUserIdentifierRequest
    {
        public int UserIdentifierTypeId { get; set; }
        public string Value { get; set; }
    }
}
