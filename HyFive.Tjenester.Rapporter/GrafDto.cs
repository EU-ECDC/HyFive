using System.Collections.Generic;

namespace HyFive.Tjenester.Rapporter
{
    public class GrafDto
    {
        public string Tittel { get; set; }
        public List<GrafDataDto> GrafDataListe { get; set; }
    }
    public class GrafDataDto
    {
        public string Name { get; set; }
        public List<PunktDto> Data { get; set; }
    }
    public class PunktDto
    {
        public string Name { get; set; }
        public decimal Y { get; set; }
    }
}
