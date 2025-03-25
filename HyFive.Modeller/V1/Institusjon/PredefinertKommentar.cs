using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institusjon
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class PredefinertKommentar
    {
        public int Id { get; set; }
        public string Kommentar { get; set; }
    }
}
