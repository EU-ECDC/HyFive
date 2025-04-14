using HyFive.Domene.Observation;
using HyFive.Models.V1.Constants;
using System;
using System.Linq;

namespace HyFive.Services.FireIndikasjoner.Helpers
{
    public class FireIndikasjonerObservasjonValidator
    {
        public static bool ValidateObservasjon(FourIndicationsObservation observasjon)
        {
            if (observasjon.IndicationTypes.Any() == false)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-01: Det må registreres minst en indikasjontype.");
            }
            if (observasjon.Activity == null)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-02: Aktivitet må registreres.");
            }
            if (observasjon.Activity.ActivityType == null)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-03: AktivitetType mangler.");
            }
            if (observasjon.Activity.TimeRecordingWasDone && observasjon.Activity.TimeSpent < 1)
            {
                throw new FireIndikasjonerObservasjonValidationException("FIO-V-04: Det er registrert at tidføring ble utført, men ingen tid ble registrert.");
            }
            if (observasjon.Role == null)
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
