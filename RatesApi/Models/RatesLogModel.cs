using System;

namespace RatesApi.Models
{
    public class RatesLogModel
    {
        public string DateTimeRequest { get; set; }
        public string DateTimeResponse { get; set; }
        public string Updated { get; set; }
        public Rates Rates { get; set; }
    }
}
