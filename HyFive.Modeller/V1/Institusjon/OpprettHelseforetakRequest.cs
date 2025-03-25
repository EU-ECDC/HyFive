using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class OpprettHelseforetakRequest
    {
        public string Navn { get; set; }
        public int RegionaltHelseforetakId { get; set; }
    }
}
