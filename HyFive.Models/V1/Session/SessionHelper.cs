using System;
using System.Collections.Generic;
using System.Text;

namespace HyFive.Models.V1.Session
{
    public class SessionHelper
    {
        public static SessionType GetSessionType(string discriminator)
        {
            switch (discriminator)
            {
                case nameof(FiveIndicationsSession):
                    return SessionType.FiveIndications;
                case nameof(HandJewelrySession):
                    return SessionType.HandJewelry;
                case nameof(ProtectiveEquipmentSession):
                    return SessionType.ProtectiveEquipment;
                case nameof(GloveSession):
                    return SessionType.Gloves;
                default:
                    return SessionType.NotSelected;
            }
        }

    }
}
