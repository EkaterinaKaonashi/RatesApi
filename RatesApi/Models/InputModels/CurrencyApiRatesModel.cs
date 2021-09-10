using System.Collections.Generic;

namespace RatesApi.Models
{
    public class CurrencyApiRatesModel
    {
        public bool Valid { get; set; }
        public double Updated { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}