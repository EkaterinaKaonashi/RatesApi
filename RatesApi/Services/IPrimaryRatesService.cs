using Exchange;

namespace RatesApi.Services
{
    public interface IPrimaryRatesService
    {
        RatesExchangeModel GetRates();
    }
}