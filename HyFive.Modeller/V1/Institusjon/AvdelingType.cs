using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class AvdelingType
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        public string Navn { get; set; }
    }
}
