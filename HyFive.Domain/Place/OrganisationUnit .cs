using HyFive.Domain.Common;
using HyFive.Domain.Observation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Place
{
    public class OrganisationUnit : AuditableEntity
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }
        public OrganisationUnit? Parent { get; set; }
        public ICollection<OrganisationUnit> Children { get; set; } 

        public string Name { get; set; } 
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        public int? TypeId { get; set; }
        public OrganisationUnitType? Type { get; set; }

        public int LevelId { get; set; }

        public OrganisationUnitLevel LevelRef { get; set; }

        public ICollection<OrganisationUnitRole> OrganisationUnitRoles { get; set; }

        public ICollection<OrganisationUnitAssociation> OutgoingAssociations { get; set; }
        public ICollection<OrganisationUnitAssociation> IncomingAssociations { get; set; }
    }
}
