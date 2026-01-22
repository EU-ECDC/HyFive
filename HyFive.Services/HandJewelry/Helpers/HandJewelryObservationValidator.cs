using HyFive.Domain.Observation;
using System;
using System.Linq;
using HyFive.Models.V1.Constants;
using HyFive.Domain.Exceptions;

namespace HyFive.Services.HandJewelry.Helpers
{
    public class HandJewelryObservationValidator
    {
        public static bool ValidateObservation(HandJewelryObservation observation)
        {
            if (observation.HandJewelries?.Any() == false)
            {
                throw new ValidationException("HandJewelryObservationMissingJewelry");
            }

            if (observation.Role == null)
            {
                throw new ValidationException("HandJewelryObservationRoleMissing");
            }

            if (observation.HandJewelries.Count > 1 &&
                observation.HandJewelries.Select(h => h.Code).Contains(HandJewelryTypeConstants.AllClear))
            {
                throw new ValidationException("HandJewelryObservationInvalidCombination", HandJewelryTypeConstants.AllClear);
            }

            return true;
        }
    }
}
