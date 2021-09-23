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
                MillisecondsDelay = 1,
                RetryTimeout = 1,
                RetryCount = 3,
                BaseCurrency = "USD",
                Currencies = new List<string> { "RUB", "EUR", "JPY" }
            };
        }

        public static CommonSettings GetCommonSettingsWithWrongCurrency()
        {
            return new CommonSettings
            {
                MillisecondsDelay = 3600000,
                RetryTimeout = 3600,
                RetryCount = 3,
                BaseCurrency = "USD",
                Currencies = new List<string> { "RUB", "EUR", "JPY", "XXX" }
            };
        }

        public static PrimaryRatesGetterSettings GetPrimaryRatesGetterSettings()
        {
            return new PrimaryRatesGetterSettings
            {
                AccessKey = "PRIMARY_ACCESS_KEY",
                Url = "https://fcurrencyapi.net/api/v1/rates?key={0}&output=JSON&base={1}"
            };
        }

        public static SecondaryRatesGetterSettings GetSecondaryRatesGetterSettings()
        {
            return new SecondaryRatesGetterSettings
            {
                AccessKey = "SECONDARY_ACCESS_KEY",
                Url = "https://fopenexchangerates.org/api/latest.json?app_id={0}&base={1}"
            };
        }
    }
}