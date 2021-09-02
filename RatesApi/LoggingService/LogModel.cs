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
        public List<string> CurrencyPairs { get; set; }
        public string delete { get; set; }


    }
}
