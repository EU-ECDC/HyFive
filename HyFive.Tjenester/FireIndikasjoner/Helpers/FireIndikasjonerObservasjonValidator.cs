using HyFive.Domene.Observasjon;
using HyFive.Modeller.V1.Konstanter;
using System;
using System.Linq;

namespace HyFive.Tjenester.FireIndikasjoner.Helpers
{
    public class FireIndikasjonerObservasjonValidator
    {
        public static bool ValidateObservasjon(FireIndikasjonerObservasjon observasjon)
        {
            if (observasjon.Indikasjonstyper.Any() == false)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-01: Det må registreres minst en indikasjontype.");
            }
            if (observasjon.Aktivitet == null)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-02: Aktivitet må registreres.");
            }
            if (observasjon.Aktivitet.AktivitetType == null)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-03: AktivitetType mangler.");
            }
            if (observasjon.Aktivitet.TidtakingBleUtfort && observasjon.Aktivitet.SekunderBrukt < 1)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-04: Det er registrert at tidføring ble utført, men ingen tid ble registrert.");
            }
            if (observasjon.Rolle == null)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-05: Rolle må registreres.");
            }

            return true;
        }
    }

    public class FireIndikasjonerObservasjonValidationException : Exception
    {
        public FireIndikasjonerObservasjonValidationException(string message) : base(message) { }
    }
}
