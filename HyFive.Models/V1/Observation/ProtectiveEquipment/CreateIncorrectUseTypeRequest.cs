using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Observation.ProtectiveEquipment
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class CreateIncorrectUseTypeRequest
    {
        public string Name { get; set; }
    }
}