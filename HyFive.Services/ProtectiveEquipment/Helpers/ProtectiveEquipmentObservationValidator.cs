using System;
using System.Linq;
using System.Text;
using HyFive.Domain.Observation.ProtectiveEquipment;

namespace HyFive.Services.ProtectiveEquipment.Helpers
{
    public class ProtectiveEquipmentObservationValidator
    {
        public static bool ValidateObservasjon(ProtectiveEquipmentObservation observation)
        {
            if (observation.ProtectiveEquipmentList?.Any(b => b.IsRequired || b.WasUsed) == false)
            {
                throw new ProtectiveEquipmentValidationException("BU-V-01: \"A protective equipment observation must have at least 1 indicated OR 1 used protective equipment registered.");
            }

            
            var equipmentErrorsCollection = new StringBuilder();
            foreach (var equipment in observation.ProtectiveEquipmentList)
            {
                if (equipment.EquipmentType == null)
                {
                    equipmentErrorsCollection.AppendLine($"BU-V-02: Equipment type must be registered on the protective equipment.");
                }

                if (equipment.WasUsed && equipment.WasUsedCorrectly == false)
                {
                    if (string.IsNullOrEmpty(equipment.Comment) && equipment.MisuseTypes?.Any() == false)
                    {
                        equipmentErrorsCollection.AppendLine("BU-V-03: If used equipment was used incorrectly, then either misuse or a comment must be registered.");
                    }
                }
            }

            var equipmentErrors = equipmentErrorsCollection.ToString();
            if (!string.IsNullOrEmpty(equipmentErrors))
            {
                throw new ProtectiveEquipmentValidationException(equipmentErrors);
            }

            if (observation.SettingType == null)
            {
                throw new ProtectiveEquipmentValidationException("BU-V-04: An observation must have a registered setting type.");
            }
            return true;
        }
        
    }
    
    public class ProtectiveEquipmentValidationException : Exception
    {
        public ProtectiveEquipmentValidationException(string message) : base(message) { }
    }
}