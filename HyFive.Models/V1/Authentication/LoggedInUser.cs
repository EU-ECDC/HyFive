using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Authentication
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class LoggedInUser
    {
        public string Id { get; set; }
        public string IdentityPseudonym { get; set; }
        public string Name { get; set; }
        public bool IsCoordinator { get; set; }
        public bool IsFhiAdmin { get; set; }
        public bool IsObserver { get; set; }
        public List<int> FacilityIds { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HPRNumber { get; set; }

        public string Email { get; set; }
    }
}