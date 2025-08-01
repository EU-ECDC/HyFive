using HyFive.Models.V1.Institution;
using Reinforced.Typings.Attributes;
using System;
using System.Collections.Generic;

namespace HyFive.Models.V1.User
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class HealthcareOrganizationCoordinator
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdentityPseudonym { get; set; }
        public bool IsDisabled { get; set; }
        public string HPRNumber { get; set; }
        public List<InstitutionReport> Institutions { get; set; }
        [TsProperty(ForceNullable = true)]
        public string ModifiedPseudonym { get; set; }
        [TsProperty(ForceNullable = true)]
        public string ModifiedHPRNumber { get; set; }

    }

}
