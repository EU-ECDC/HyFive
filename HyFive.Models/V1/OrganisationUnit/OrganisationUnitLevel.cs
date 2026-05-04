using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class OrganisationUnitLevel
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
    }
}
