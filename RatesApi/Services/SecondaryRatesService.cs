using AutoMapper;
using Exchange;
using Microsoft.Extensions.Options;
using RatesApi.RatesGetters;
using RatesApi.RatesGetters.Deserializers;
using RatesApi.Settings;

namespace RatesApi.Services
{
    public class SecondaryRatesService : ISecondaryRatesService
    {
        private readonly IRatesGetter _ratesGetter;

        public SecondaryRatesService(
            IMapper mapper, IRatesGetter ratesGetter,
            IOptions<SecondaryRatesGetterSettings> settings)
        {
            _ratesGetter = ratesGetter;
            _ratesGetter.ConfigureGetter(new OpenExchangeRatesResponseDeserializer(), settings.Value);
        }

        public RatesExchangeModel GetRates()
        {
            return _ratesGetter.GetRates();
        }
    }
}
