using System.Collections.Generic;

namespace RatesApi.Models.InputModels
{
    public class OpenExchangeRatesModel
    {
        public int Timestamp { get; set; }
        public string Base { get; set; }
        public Dictionary<string,decimal> Rates { get; set; }
    }
}
