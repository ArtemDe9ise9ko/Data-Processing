namespace DataReederConsole.Models
{
    public class CityData
    {
        public string City { get; set; } = null!;
        public List<Service> Services { get; set; } = null!;
        public decimal Total { get; set; }
    }
}
