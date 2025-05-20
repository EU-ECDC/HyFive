using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class PredefinedComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
    }
}
