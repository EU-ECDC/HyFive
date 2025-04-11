using HyFive.Domene.Observation;
using System;
using System.Linq;
using HyFive.Modeller.V1.Konstanter;

namespace HyFive.Tjenester.Handsmykke.Helpers
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
            
            if(observasjon.HandJewelry.Count > 1 && observasjon.HandJewelry.Select(h => h.Code).Contains(HandsmykkeTypeKonstanter.AltOk))
            {
                throw new HandsmykkeObservasjonValidationException($"HO-V-03: '{HandsmykkeTypeKonstanter.AltOk}' kan ikke kombineres med andre typer håndsmykker'");
            }
                

            return true;
        }
    }
    
    public class HandsmykkeObservasjonValidationException : Exception
    {
        public HandsmykkeObservasjonValidationException(string message) : base(message) { }
    }
}
