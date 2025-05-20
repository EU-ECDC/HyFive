using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.User
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class User
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdentityPseudonym { get; set; }
        public bool IsDisabled { get; set; }
        public string HPRNumber { get; set; }
       
    }
}
