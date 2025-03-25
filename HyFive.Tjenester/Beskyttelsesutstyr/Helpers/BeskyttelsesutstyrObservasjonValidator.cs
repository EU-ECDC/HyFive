using System;
using System.Linq;
using System.Text;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;

namespace HyFive.Tjenester.Beskyttelsesutstyr.Helpers
{
    public class BeskyttelsesutstyrObservasjonValidator
    {
        public static bool ValidateObservasjon(BeskyttelsesutstyrObservasjon observasjon)
        {
            if (observasjon.Beskyttelsesutstyrliste?.Any(b => b.ErIndikert || b.BleBenyttet) == false)
            {
                throw new BeskyttelsesutstyrValidationException("BU-V-01: En Beskyttelesutstyr-observasjon må ha minst 1 indikert ELLER 1 benyttet beskyttelsesutstyr registrert");
            }

            
            var utstyrsfeilsamling = new StringBuilder();
            foreach (var utstyr in observasjon.Beskyttelsesutstyrliste)
            {
                if (utstyr.Utstyrstype == null)
                {
                    utstyrsfeilsamling.AppendLine($"BU-V-02: Utstyrstype må være registrert på Beskyttelsesutstyret");
                }

                if (utstyr.BleBenyttet && utstyr.BleBenyttetRiktig == false)
                {
                    if (string.IsNullOrEmpty(utstyr.Kommentar) && utstyr.Feilbruktyper?.Any() == false)
                    {
                        utstyrsfeilsamling.AppendLine("BU-V-03: Hvis benyttet utstyr ble benyttet feil så må enten feilbruk eller kommentar registreres.");
                    }
                }
            }

            var utstyrsfeil = utstyrsfeilsamling.ToString();
            if (!string.IsNullOrEmpty(utstyrsfeil))
            {
                throw new BeskyttelsesutstyrValidationException(utstyrsfeil);
            }

            if (observasjon.Settingtype == null)
            {
                throw new BeskyttelsesutstyrValidationException("BU-V-04: En observasjon må ha en registrert Settingtype");
            }
            return true;
        }
        
    }
    
    public class BeskyttelsesutstyrValidationException : Exception
    {
        public BeskyttelsesutstyrValidationException(string message) : base(message) { }
    }
}