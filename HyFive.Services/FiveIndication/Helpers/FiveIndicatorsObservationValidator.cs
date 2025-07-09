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
            if (observation.IndicationTypes.Any() == false)
            {
                throw new FiveIndicatorsObservationValidationException("FIO-V-01: At least one indicator type must be registered.");
            }
            if (observation.Activity == null)
            {
                throw new FiveIndicatorsObservationValidationException("FIO-V-02: Activity must be registered.");
            }
            if (observation.Activity.ActivityType == null)
            {
                throw new FiveIndicatorsObservationValidationException("FIO-V-03: ActivityType is missing.");
            }
            if (observation.Activity.TimingWasPerformed && observation.Activity.SecondsUsed < 1)
            {
                throw new FiveIndicatorsObservationValidationException("FIO-V-04: It is recorded that time tracking was performed, but no time was recorded.");
            }
            if (observation.Role == null)
            {
                throw new FiveIndicatorsObservationValidationException("FIO-V-05: Role must be registered.");
            }

            return true;
        }
    }

    public class FiveIndicatorsObservationValidationException : Exception
    {
        public FiveIndicatorsObservationValidationException(string message) : base(message) { }
    }
}
