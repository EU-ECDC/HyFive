using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class NotCompletedActivityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}