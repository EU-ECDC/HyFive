using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Observasjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class ActivityType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}