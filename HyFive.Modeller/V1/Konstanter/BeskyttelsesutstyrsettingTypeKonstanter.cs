using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public class BeskyttelsesutstyrsettingTypeKonstanter
    {
        [TsProperty(Constant = true)]
        public static string BasaleSmittevernrutiner =  "BASALE_SMITTEVERNRUTINER";

        [TsProperty(Constant = true)]
        public static string Kontaktsmitte = "KONTAKTSMITTE";
        
        [TsProperty(Constant = true)]
        public static string Drapesmitte = "DRAPESMITTE";

        [TsProperty(Constant = true)]
        public static string Luftsmitte = "LUFTSMITTE";
    }
}
