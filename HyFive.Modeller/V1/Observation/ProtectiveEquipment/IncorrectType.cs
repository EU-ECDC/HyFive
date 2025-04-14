using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.ProtectiveEquipment
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class IncorrectType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}