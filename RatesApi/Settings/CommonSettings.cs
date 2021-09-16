using System.Collections.Generic;

namespace RatesApi.Settings
{
    public class CommonSettings
    {
        
        public int RetryTimeout { get; set; }
        public int RetryCount { get; set; }
        public int MillisecondsDelay { get; set; }
        public string BaseCurrency { get; set; }
        public List<string> Currencies { get; set; }
    }
}
