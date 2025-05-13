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
                case nameof(FourIndicationsSession):
                    return SessionType.FourIndications;
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
