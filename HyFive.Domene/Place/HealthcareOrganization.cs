namespace HyFive.Domain.Place
{
    public class HealthcareOrganization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RegionaltHealthcareProvider RegionaltHealthcareOrganization { get; set; }
    }
}
