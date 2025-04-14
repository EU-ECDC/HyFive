using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.User
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateFhiAdminRequest
    {
        public string IdentityPseudonym { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
    }
}
