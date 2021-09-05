using System;

namespace RatesApi.Models
{
    public class RatesOutputModel
    {
        public DateTime Updated { get; set; }
        public string BaseCurrency { get; set; }
        public Rates Rates { get; set; }
    }
}