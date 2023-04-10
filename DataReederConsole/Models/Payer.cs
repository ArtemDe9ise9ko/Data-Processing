namespace DataReederConsole.Models
{
    public class Payer
    {
        public string Name { get; set; } = null!;
        public decimal Payment { get; set; }
        public DateTime Date { get; set; }
        public long AccountNumber { get; set; }
    }
}
