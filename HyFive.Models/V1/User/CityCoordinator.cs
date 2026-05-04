using HyFive.Models.V1.OrganisationUnit;
using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;

namespace HyFive.Models.V1.User
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CityCoordinator
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdentityPseudonym { get; set; }
        public bool IsDeactivated { get; set; }
        public List<FacilityReport> Facilities { get; set; }
        [TsProperty(ForceNullable = true)]
        public string ModifiedPseudonym { get; set; }

        [TsProperty(ForceNullable = true)]
        public List<UserIdentifier> UserIdentifiers { get; set; }

        [TsProperty(ForceNullable = true)]
        public List<UpdateUserIdentifierRequest> ModifiedIdentifiers { get; set; }

    }

}
