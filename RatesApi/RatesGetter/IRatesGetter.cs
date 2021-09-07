using RatesApi.Models;

namespace RatesApi.RatesGetter
{
    public interface IRatesGetter
    {
        CurrencyApiRatesModel GetRates();
    }
}