using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class OrganisationUnit
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public int LevelId { get; set; }
        public OrganisationUnitLevel LevelRef { get; set; }

        public int TypeId { get; set; }
        public OrganisationUnitType Type { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        // Only return this when you explicitly return a tree response
        public List<OrganisationUnit> Children { get; set; }

        public List<Models.V1.Observation.Role> Roles { get; set; }

        public bool HasObservations { get; set; }
    }
}
