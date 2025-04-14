using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Institution
{
    [TsInterface(IncludeNamespace = false, AutoI = false)]
    public class Comment
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
    }
}
