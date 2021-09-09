using System.Collections.Generic;

namespace RatesApi.Settings
{
    public class RatesGetterSettings
    {
        public string PrimaryAccessKey { get; set; }
        public string PrimaryUrl { get; set; }
        public string BaseCurrency { get; set; }
        public List<string> Currencies { get; set; }
    }
}
