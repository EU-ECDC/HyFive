namespace HyFive.Domain.Place
{
    public class HealthcareOrganization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RegionaltHealthcareOrganization RegionalHealthcareOrganization { get; set; }
    }
}
