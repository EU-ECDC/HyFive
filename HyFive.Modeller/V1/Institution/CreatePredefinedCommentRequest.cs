using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreatePredefinedCommentRequest
    {
        public string Comment { get; set; }
    }
}
