using System.Collections.Generic;

namespace RatesApi.Models
{
    public class CurrencyApiRatesModel
    {
        public double Updated { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}