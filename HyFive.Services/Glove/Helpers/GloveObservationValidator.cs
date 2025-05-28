using System;
using System.Linq;
using HyFive.Domain.Observation.Gloves;

namespace HyFive.Services.Glove.Helpers
{
    public class GloveObservationValidator
    {
        public static bool ValidateObservation(GloveObservation observation)
        {
            if (observation.GloveUsed == false && observation.PostGloveHandHygieneType != null)
            {
                throw new GloveObservationValidationException("H-V-02: If gloves were not used, 'Hand hygiene after glove use' should not be answered");
            }

            if (observation.IndicatedGloveTypes?.Any() == true && observation.GloveWithoutIndicationTypes?.Any() == true)
            {
                throw new GloveObservationValidationException("H-V-03: One cannot register both 'With indication' and 'Without indication' at the same time");
            }
            
            if (observation.IndicatedGloveTypes?.Any() == false && observation.GloveWithoutIndicationTypes?.Any() == false)
            {
                throw new GloveObservationValidationException("H-V-04: At least 1 indication type (With/Without) must be registered");
            }
            
            if (observation.GloveUsed == false && observation.GloveWithoutIndicationTypes?.Any() == true )
            {
                throw new GloveObservationValidationException("H-V-05: If gloves were not used, 'GloveWithoutIndicationTypes' should not be registered");
            }
            
            if (observation.Role == null)
            {
                throw new GloveObservationValidationException("H-V-06: Role må registreres.");
            }

            return true;
        }
    }
    
    public class GloveObservationValidationException : Exception
    {
        public GloveObservationValidationException(string message) : base(message) { }
    }
}
