using HyFive.Domain.Observation;
using HyFive.Models.V1.Constants;
using System;
using System.Linq;

namespace HyFive.Services.FireIndikasjoner.Helpers
{
    public class FourIndicatorsObservationValidator
    {
        public static bool ValidateObservation(FourIndicationsObservation observation)
        {
            if (observation.IndicationTypes.Any() == false)
            {
                throw new FourIndicatorsObservationValidationException("FIO-V-01: At least one indicator type must be registered.");
            }
            if (observation.Activity == null)
            {
                throw new FourIndicatorsObservationValidationException("FIO-V-02: Activity must be registered.");
            }
            if (observation.Activity.ActivityType == null)
            {
                throw new FourIndicatorsObservationValidationException("FIO-V-03: ActivityType is missing.");
            }
            if (observation.Activity.TimeRecordingWasDone && observation.Activity.TimeSpent < 1)
            {
                throw new FourIndicatorsObservationValidationException("FIO-V-04: It is recorded that time tracking was performed, but no time was recorded.");
            }
            if (observation.Role == null)
            {
                throw new FourIndicatorsObservationValidationException("FIO-V-05: Role must be registered.");
            }

            return true;
        }
    }

    public class FourIndicatorsObservationValidationException : Exception
    {
        public FourIndicatorsObservationValidationException(string message) : base(message) { }
    }
}
