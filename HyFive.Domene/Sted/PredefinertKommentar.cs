
namespace HyFive.Domene.Sted
{
    public class PredefinertKommentar
    {
        public int Id { get; set; }
        public string Kommentar { get; set; }
        public SesjonType SesjonType { get; set; }
        public int InstitusjonId { get; set; }
    }

    public enum SesjonType
    {
        Beskyttelsesutstyr = 5
    }
}
