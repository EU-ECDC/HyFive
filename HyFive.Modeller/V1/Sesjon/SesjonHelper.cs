using System;
using System.Collections.Generic;
using System.Text;

namespace HyFive.Modeller.V1.Sesjon
{
    public class SesjonHelper
    {
        public static SesjonType HentSesjonType(string discriminator)
        {
            switch (discriminator)
            {
                case nameof(FireIndikasjonerSesjon):
                    return SesjonType.FireIndikasjoner;
                case nameof(HandsmykkeSesjon):
                    return SesjonType.Handsmykker;
                case nameof(BeskyttelsesutstyrSesjon):
                    return SesjonType.Beskyttelsesutstyr;
                case nameof(HanskeSesjon):
                    return SesjonType.Hansker;
                default:
                    return SesjonType.IkkeValgt;
            }
        }
    }
}
