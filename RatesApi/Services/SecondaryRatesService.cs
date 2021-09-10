using AutoMapper;
using Exchange;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatesApi.RatesGetters;
using RatesApi.RatesGetters.ResponceParsers;
using RatesApi.Settings;

namespace RatesApi.Services
{
    public class SecondaryRatesService : ISecondaryRatesService
    {
        private readonly ILogger<SecondaryRatesService> _logger;
        private readonly IRatesGetter _ratesGetter;

        public SecondaryRatesService(
            ILogger<SecondaryRatesService> logger,
            IMapper mapper, IRatesGetter ratesGetter,
            IOptions<SecondaryRatesGetterSettings> settings)
        {
            _logger = logger;
            _ratesGetter = ratesGetter;
            _ratesGetter.ConfigureGetter(new CurrencyApiResponceParser(mapper), settings.Value);
        }

        public RatesExchangeModel GetRates()
        {
            return _ratesGetter.GetRates();
        }
    }
}
