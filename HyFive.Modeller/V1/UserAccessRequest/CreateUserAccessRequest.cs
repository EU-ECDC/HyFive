using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.ForesporselOmBrukertilgang
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateUserAccessRequest
    {
        [TsProperty(ForceNullable = true)]
        public int InstitutionId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string IdentityPseudonym { get; set; }
        public string HPRNumber { get; set; }
    }
}
