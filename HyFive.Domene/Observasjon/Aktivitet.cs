
namespace HyFive.Domene.Observasjon
{
    public class Aktivitet
    {
        public int Id { get; set; }

        public int SekunderBrukt { get; set; }

        public bool TidtakingBleUtfort { get; set; }
        
        public bool? BenyttetHanske { get; set; }

        public AktivitetType AktivitetType { get; set; }
    }
}
