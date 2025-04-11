
namespace HyFive.Domain.Place
{
    public class PredefinedComments
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public SessionType SessionType { get; set; }
        public int InstitutionId { get; set; }
    }

    public enum SessionType
    {
        ProtectiveEquipment = 5
    }
}
