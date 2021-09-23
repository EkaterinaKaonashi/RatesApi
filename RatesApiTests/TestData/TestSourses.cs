using System.Collections;

namespace RatesApiTests.TestData
{
    public static class TestSourses
    {
        public static IEnumerable RatesGetterSourse()
        {
            yield return new object[] { RatesModels.GetCurrencyApiRatesModel(), SettingsData.GetPrimaryRatesGetterSettings() };
            yield return new object[] { RatesModels.GetOpenExchangeRatesModel(), SettingsData.GetSecondaryRatesGetterSettings() };
        }
    }
}
