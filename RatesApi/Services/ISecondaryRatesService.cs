using Exchange;

namespace RatesApi.Services
{
    public interface ISecondaryRatesService
    {
        RatesExchangeModel GetRates();
    }
}