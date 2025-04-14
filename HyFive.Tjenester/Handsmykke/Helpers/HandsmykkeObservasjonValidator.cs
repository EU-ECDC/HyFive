using HyFive.Domain.Observation;
using System;
using System.Linq;
using HyFive.Models.V1.Constants;

namespace HyFive.Services.Handsmykke.Helpers
{
    public class HandsmykkeObservasjonValidator
    {
        public static bool ValidateObservasjon(HandJewelryObservation observasjon)
        {
            if (observasjon.HandJewelry?.Any() == false)
            {
                throw new HandsmykkeObservasjonValidationException("HO-V-01: Minst 1 håndsmykke må registreres");
            }
            
            if (observasjon.Role == null)
            {
                throw new HandsmykkeObservasjonValidationException("HO-V-02: Rolle må registreres.");
            }
            
            if(observasjon.HandJewelry.Count > 1 && observasjon.HandJewelry.Select(h => h.Code).Contains(HandJewelryTypeConstants.AllClear))
            {
                throw new HandsmykkeObservasjonValidationException($"HO-V-03: '{HandJewelryTypeConstants.AllClear}' kan ikke kombineres med andre typer håndsmykker'");
            }
                

            return true;
        }
    }
    
    public class HandsmykkeObservasjonValidationException : Exception
    {
        public HandsmykkeObservasjonValidationException(string message) : base(message) { }
    }
}
