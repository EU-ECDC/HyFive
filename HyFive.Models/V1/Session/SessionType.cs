using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
{

    [TsEnum(IncludeNamespace = false)]
    public enum SessionType {
        FiveIndications = 1,
        InOut = 2,
        HandJewelry = 3,
        Gloves = 4,
        ProtectiveEquipment = 5,
        NotSelected = 6
    }
}
