using System;
using System.Linq;
using System.Text;
using HyFive.Domene.Observation.ProtectiveEquipment;

namespace HyFive.Tjenester.Beskyttelsesutstyr.Helpers
{
    public class BeskyttelsesutstyrObservasjonValidator
    {
        public static bool ValidateObservasjon(ProtectiveEquipmentObservation observasjon)
        {
            if (observasjon.ProtectiveEquipmentList?.Any(b => b.IsRequired || b.WasUsed) == false)
            {
                throw new BeskyttelsesutstyrValidationException("BU-V-01: En Beskyttelesutstyr-observasjon må ha minst 1 indikert ELLER 1 benyttet beskyttelsesutstyr registrert");
            }

            
            var utstyrsfeilsamling = new StringBuilder();
            foreach (var utstyr in observasjon.ProtectiveEquipmentList)
            {
                if (utstyr.EquipmentType == null)
                {
                    utstyrsfeilsamling.AppendLine($"BU-V-02: Utstyrstype må være registrert på Beskyttelsesutstyret");
                }

                if (utstyr.WasUsed && utstyr.WasUsedCorrectly == false)
                {
                    if (string.IsNullOrEmpty(utstyr.Comment) && utstyr.MisuseTypes?.Any() == false)
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

            if (observasjon.SettingType == null)
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