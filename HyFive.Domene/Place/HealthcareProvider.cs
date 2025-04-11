namespace HyFive.Domain.Place
{
    public class HealthcareProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RegionaltHealthcareProvider RegionaltHealthcareProvider { get; set; }
    }
}
