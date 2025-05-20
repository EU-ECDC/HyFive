using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class InstitutionType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
