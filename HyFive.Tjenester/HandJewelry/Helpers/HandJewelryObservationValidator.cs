using HyFive.Domain.Observation;
using System;
using System.Linq;
using HyFive.Models.V1.Constants;

namespace HyFive.Services.HandJewelry.Helpers
{
    public class HandJewelryObservationValidator
    {
        public static bool ValidateObservation(HandJewelryObservation observation)
        {
            if (observation.HandJewelry?.Any() == false)
            {
                throw new HandJewelryObservationValidationException("HO-V-01: At least 1 hand jewelry must be registered");
            }
            
            if (observation.Role == null)
            {
                throw new HandJewelryObservationValidationException("HO-V-02: Role must be registered.");
            }
            
            if(observation.HandJewelry.Count > 1 && observation.HandJewelry.Select(h => h.Code).Contains(HandJewelryTypeConstants.AllClear))
            {
                throw new HandJewelryObservationValidationException($"HO-V-03: '{HandJewelryTypeConstants.AllClear}' cannot be combined with other types of  hand jewelries'");
            }
                

            return true;
        }
    }
    
    public class HandJewelryObservationValidationException : Exception
    {
        public HandJewelryObservationValidationException(string message) : base(message) { }
    }
}
