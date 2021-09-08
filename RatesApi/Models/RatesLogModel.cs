using System.Collections.Generic;

namespace RatesApi.Models
{
    public class RatesLogModel
    {
        public string DateTimeRequest { get; set; }
        public string DateTimeResponse { get; set; }
        public string BaseCurrency { get; set; }
        public string Updated { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
