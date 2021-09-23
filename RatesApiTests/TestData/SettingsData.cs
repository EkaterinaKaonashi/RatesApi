using RatesApi.Settings;
using System.Collections.Generic;

namespace RatesApiTests.TestData
{
    public static class SettingsData
    {
        public static CommonSettings GetCommonSettings()
        {
            return new CommonSettings
            {
                MillisecondsDelay = 3600000,
                RetryTimeout = 3600,
                RetryCount = 3,
                BaseCurrency = "USD",
                Currencies = new List<string> { "RUB", "EUR", "JPY" }
            };
        }
    }
}
