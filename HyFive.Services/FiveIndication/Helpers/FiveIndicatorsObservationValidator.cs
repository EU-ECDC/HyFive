using HyFive.Domain.Exceptions;
using HyFive.Domain.Observation;
using HyFive.Models.V1.Constants;
using System;
using System.Linq;

namespace HyFive.Services.FiveIndication.Helpers
{
    public class FiveIndicatorsObservationValidator
    {
        public static bool ValidateObservation(FiveIndicationsObservation observation)
        {
            if (observation.IndicationTypes == null || observation.IndicationTypes?.Count == 0)
            {
                throw new ValidationException("ObservationIndicatorTypeMissing");
            }

            if (observation.Activity == null)
            {
                throw new ValidationException("ObservationActivityMissing");
            }

            if (observation.Activity.ActivityType == null)
            {
                throw new ValidationException("ObservationActivityTypeMissing");
            }

            if (observation.Activity.TimingWasPerformed && observation.Activity.SecondsUsed < 1)
            {
                throw new ValidationException("ObservationTimingInvalid");
            }

            if (observation.Role == null)
            {
                throw new ValidationException("ObservationRoleMissing");
            }

            return true;
        }
    }
}
