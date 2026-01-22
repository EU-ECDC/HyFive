using System;
using System.Linq;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Observation.Gloves;

namespace HyFive.Services.Glove.Helpers
{
    public class GloveObservationValidator
    {
        public static bool ValidateObservation(GloveObservation observation)
        {
            if (!observation.GlovesUsed  && observation.PostGloveHandHygieneType != null)
            {
                throw new ValidationException("GloveObservationInvalidPostHygiene");
            }

            if ((observation.GloveWithIndicationTypes?.Count > 0) && (observation.GloveWithoutIndicationTypes?.Count > 0))
            {
                throw new ValidationException("GloveObservationWithAndWithoutConflict");
            }

            if ((observation.GloveWithIndicationTypes?.Count > 0 ) && (observation.GloveWithoutIndicationTypes?.Count > 0))
            {
                throw new ValidationException("GloveObservationMissingIndication");
            }

            if (!observation.GlovesUsed && (observation.GloveWithoutIndicationTypes?.Count > 0))
            {
                throw new ValidationException("GloveObservationInvalidWithoutUsage");
            }

            if (observation.Role == null)
            {
                throw new ValidationException("GloveObservationRoleMissing");
            }

            return true;
        }
    }
}
