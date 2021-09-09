using Exchange;

namespace RatesApi.RatesGetters
{
    public interface IRatesGetter
    {
        RatesExchangeModel GetRates();
    }
}