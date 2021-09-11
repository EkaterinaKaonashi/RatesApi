using Exchange;
using Microsoft.Extensions.Options;
using RatesApi.Models;
using RatesApi.RatesGetters;
using RatesApi.Settings;

namespace RatesApi.Services
{
    public class PrimaryRatesService : IPrimaryRatesService
    {
        private readonly IRatesGetter _ratesGetter;

        public PrimaryRatesService(IRatesGetter ratesGetter,
            IOptions<PrimaryRatesGetterSettings> settings)
        {
            _ratesGetter = ratesGetter;
            _ratesGetter.ConfigureGetter(settings.Value);
        }

        public RatesExchangeModel GetRates() => _ratesGetter.GetRates<CurrencyApiRatesModel>();
    }
}
