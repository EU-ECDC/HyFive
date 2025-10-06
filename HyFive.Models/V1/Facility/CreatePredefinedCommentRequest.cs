using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Facility
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreatePredefinedCommentRequest
    {
        public string Comment { get; set; }
    }
}
