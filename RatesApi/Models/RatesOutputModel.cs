using System.Collections.Generic;

namespace RatesApi.Models
{
    public class RatesOutputModel
    {
        public string Updated { get; set; }
        public string BaseCurrency { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}