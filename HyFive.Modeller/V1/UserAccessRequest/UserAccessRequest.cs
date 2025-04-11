using System;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.ForesporselOmBrukertilgang
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class UserAccessRequest
    {
        public int Id { get; set; }
        [TsProperty(ForceNullable = true)]
        public int InstitutionId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string IdentityPseudonym { get; set; }
        public string HPRNumber { get; set; }
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ApprovedTime { get; set; }
    }
}
