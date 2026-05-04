using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreateOrganisationUnitLevelRequest
    {
        public string Level { get; set; }
        public string Description { get; set; }
    }
}
