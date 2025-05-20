using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class Status
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
