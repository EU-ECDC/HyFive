using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.OrganisationUnit
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class CreatePredefinedCommentRequest
    {
        public string Comment { get; set; }
    }
}
