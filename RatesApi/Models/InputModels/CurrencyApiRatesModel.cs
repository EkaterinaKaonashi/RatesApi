namespace RatesApi.Models
{
    public class CurrencyApiRatesModel
    {
        public bool Valid { get; set; }
        public double Updated { get; set; }
        public string Base { get; set; }
        public CurrencyApiRates Rates { get; set; }
    }
}