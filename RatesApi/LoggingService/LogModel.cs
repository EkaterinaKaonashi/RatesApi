using RatesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatesApi.LoggingService
{
    public class LogModel
    {
        public DateTime DateTimeRequest { get; set; }
        public DateTime DateTimeResponse { get; set; }
        public string BaseCurrency { get; set; }
        public Rates CurrencyRates { get; set; }
    }
}
