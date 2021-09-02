namespace RatesApi.Models
{
    public class RatesInputModel
    {
        public bool Success { get; set; }
        public string Privacy { get; set; }
        public double Timestamp { get; set; }
        public string Source { get; set; }
        public Quotes Quotes { get; set; }
    }
}