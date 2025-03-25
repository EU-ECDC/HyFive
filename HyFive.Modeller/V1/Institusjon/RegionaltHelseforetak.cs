using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class RegionaltHelseforetak
    {
        public int Id { get; set; }
        public string Navn { get; set; }
    }
}
