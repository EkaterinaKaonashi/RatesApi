using System;

namespace RatesApi.Models
{
    public class RatesOutputModel
    {
        public DateTime DateTime { get; set; }
        public string Source { get; set; }
        public Quotes Quotes { get; set; }
    }
}