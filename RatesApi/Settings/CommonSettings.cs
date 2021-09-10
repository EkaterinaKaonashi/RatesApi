using System.Collections.Generic;

namespace RatesApi.Settings
{
    public class CommonSettings
    {
        public string AdminEmail { get; set; }
        public string BaseCurrency { get; set; }
        public List<string> Currencies { get; set; }
    }
}
