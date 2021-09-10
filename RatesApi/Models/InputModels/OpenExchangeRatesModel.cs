using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatesApi.Models.InputModels
{
    public class OpenExchangeRatesModel
    {
        public int Timestamp { get; set; }
        public string Base { get; set; }
        public Dictionary<string,decimal> Rates { get; set; }
    }
}
