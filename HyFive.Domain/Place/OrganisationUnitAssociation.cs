using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Place
{
    public class OrganisationUnitAssociation
    {
        public int Id { get; set; }

        public int SourceOrganisationUnitId { get; set; }
        public OrganisationUnit SourceOrganisationUnit { get; set; }

        public int TargetOrganisationUnitId { get; set; }
        public OrganisationUnit TargetOrganisationUnit { get; set; }

        public OrganisationUnitAssociationType AssociationType { get; set; }
    }

    public enum OrganisationUnitAssociationType
    {
        UnitDepartment = 1,
        FacilityDepartment = 2
    }
}
