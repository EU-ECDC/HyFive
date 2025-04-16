using System;
using System.Linq;
using HyFive.Domene.Observation.Gloves;

namespace HyFive.Services.Glove.Helpers
{
    public class HanskeObservasjonValidator
    {
        public static bool ValidateObservasjon(GloveObservation observasjon)
        {
            if (observasjon.GloveUsed == false && observasjon.HandhygieneEtterHanskebrukType != null)
            {
                throw new HanskeObservasjonValidationException("H-V-02: Hvis hansker _ikke_ ble benyttet så skal _ikke_ 'Håndhygiene etter hanskebruk' besvares");
            }

            if (observasjon.IndicatedGloveTypes?.Any() == true && observasjon.GeneralPurposeGloveTypes?.Any() == true)
            {
                throw new HanskeObservasjonValidationException("H-V-03: En kan ikke registrere både 'Med indikasjon' og 'Uten indikasjon' samtidig ");
            }
            
            if (observasjon.IndicatedGloveTypes?.Any() == false && observasjon.GeneralPurposeGloveTypes?.Any() == false)
            {
                throw new HanskeObservasjonValidationException("H-V-04: En må registrere minst 1 indikasjonstype (Med/Uten)");
            }
            
            if (observasjon.GloveUsed == false && observasjon.GeneralPurposeGloveTypes?.Any() == true )
            {
                throw new HanskeObservasjonValidationException("H-V-05: Hvis hansker ikke ble benyttet så skal ikke HanskeUtenIndikasjonTyper registreres ");
            }
            
            if (observasjon.Role == null)
            {
                throw new HanskeObservasjonValidationException("H-V-06: Rolle må registreres.");
            }

            return true;
        }
    }
    
    public class HanskeObservasjonValidationException : Exception
    {
        public HanskeObservasjonValidationException(string message) : base(message) { }
    }
}
