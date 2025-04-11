using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.ForesporselOmBrukertilgang
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class InstitutionForUserAccessRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
