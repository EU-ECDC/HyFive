using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsEnum(IncludeNamespace = false)]
    public enum NotCompletedActivityTypeId
    {
        NotCompleted = 1,
        GloveWasUsed = 2,
        GloveWasNotUsed = 3
    }
}