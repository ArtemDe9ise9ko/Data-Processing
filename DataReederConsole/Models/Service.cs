namespace DataReederConsole.Models
{
    public class Service
    {
        public string Name { get; set; } = null!;
        public List<Payer> Payers { get; set; } = null!;
        public decimal Total { get; set; }
    }
}
