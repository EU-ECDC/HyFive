using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public class BeskyttelsesutstyrTypeKonstanter
    {
        [TsProperty(Constant = true)]
        public static string Hansker =  "HANSKER";

        [TsProperty(Constant = true)]
        public static string Smittefrakk = "SMITTEFRAKK";

        [TsProperty(Constant = true)]
        public static string Munnbind = "MUNNBIND";
        
        [TsProperty(Constant = true)]
        public static string Oyebeskyttelse = "OYEBESKYTTELSE";

        [TsProperty(Constant = true)]
        public static string Andedrettsvern = "ANDEDRETTSVERN";

        [TsProperty(Constant = true)]
        public static string Hette = "HETTE";

        [TsProperty(Constant = true)]
        public static string Plastforkle = "PLASTFORKLE";

        [TsProperty(Constant = true)]
        public static string Stellefrakk = "STELLEFRAKK";
    }
}
