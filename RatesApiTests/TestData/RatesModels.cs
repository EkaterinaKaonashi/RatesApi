using Exchange;
using RatesApi.Models;
using RatesApi.Models.InputModels;
using System.Collections.Generic;

namespace RatesApiTests.TestData
{
    public static class RatesModels
    {
        public static CurrencyApiRatesModel GetCurrencyApiRatesModel()
        {
            return new CurrencyApiRatesModel
            {
                Updated = 1632387604,
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "RUB", 72.6963m },
                    { "EUR", 0.85355m },
                    { "JPY", 109.937m },
                    { "GBP", 0.74122m },
                    { "CHF", 0.93234m }
                }
            };
        }

        public static OpenExchangeRatesModel GetOpenExchangeRatesModel()
        {
            return new OpenExchangeRatesModel
            {
                Timestamp = 1632387604,
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "RUB", 72.6963m },
                    { "EUR", 0.85355m },
                    { "JPY", 109.937m },
                    { "GBP", 0.74122m },
                    { "CHF", 0.93234m }
                }
            };
        }

        public static RatesExchangeModel GetRatesExchangeModel()
        {
            return new RatesExchangeModel
            {
                Updated = "23.09.2021 12:00:04",
                BaseCurrency = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "USDRUB", 72.6963m },
                    { "USDEUR", 0.85355m },
                    { "USDJPY", 109.937m }
                }
            };
        }
    }
}
