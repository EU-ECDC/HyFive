using System;
using System.Linq;
using System.Text;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Observation.ProtectiveEquipment;

namespace HyFive.Services.ProtectiveEquipment.Helpers
{
    public class ProtectiveEquipmentObservationValidator
    {
        public static bool ValidateObservation(ProtectiveEquipmentObservation observation)
        {
            if (observation.ProtectiveEquipmentList?.Any(b => b.IsRequired || b.WasUsed) == false)
            {
                throw new ValidationException("ProtectiveEquipmentObservationMissingUsage");
            }

            foreach (var equipment in observation.ProtectiveEquipmentList)
            {
                if (equipment.EquipmentType == null)
                {
                    throw new ValidationException("ProtectiveEquipmentObservationMissingType");
                }

                if (equipment.WasUsed &&
                    !equipment.WasUsedCorrectly &&
                    string.IsNullOrEmpty(equipment.Comment) &&
                    (equipment.MisuseTypes?.Count ?? 0) == 0)
                {
                    throw new ValidationException("ProtectiveEquipmentObservationInvalidMisuse");
                }
            }

            if (observation.SettingType == null)
            {
                throw new ValidationException("ProtectiveEquipmentObservationMissingSettingType");
            }

            return true;
        }

    }
}