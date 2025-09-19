using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.UserAccessRequest
{
    [TsInterface(AutoI = false, IncludeNamespace = false)]
    public class FacilityForUserAccessRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
