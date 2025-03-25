using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Helseforetak
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public int RegionaltHelseforetakId { get; set; }
        public RegionaltHelseforetak RegionaltHelseforetak { get; set; }
    }
}
