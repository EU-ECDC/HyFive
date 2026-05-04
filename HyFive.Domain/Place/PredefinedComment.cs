
namespace HyFive.Domain.Place
{
    public class PredefinedComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public SessionType SessionType { get; set; }
        public int OrganisationUnitId { get; set; }
        public OrganisationUnit OrganisationUnit { get; set; }
    }

    public enum SessionType
    {
        ProtectiveEquipment = 5
    }
}
