using System;
using System.Collections.Generic;
using System.Text;

namespace HyFive.Models.V1.Session
{
    public class SesjonHelper
    {
        public static SesjonType HentSesjonType(string discriminator)
        {
            switch (discriminator)
            {
                case nameof(FourIndicationsSession):
                    return SesjonType.FireIndikasjoner;
                case nameof(HandJewelrySession):
                    return SesjonType.Handsmykker;
                case nameof(ProtectiveEquipmentSession):
                    return SesjonType.Beskyttelsesutstyr;
                case nameof(HanskeSesjon):
                    return SesjonType.Hansker;
                default:
                    return SesjonType.IkkeValgt;
            }
        }
    }
}
