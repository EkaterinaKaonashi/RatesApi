using Exchange;
using Microsoft.Extensions.Options;
using RatesApi.Models.InputModels;
using RatesApi.RatesGetters;
using RatesApi.Settings;

namespace RatesApi.Services
{
    public class SecondaryRatesService : ISecondaryRatesService
    {
        private readonly IRatesGetter _ratesGetter;

        public SecondaryRatesService(IRatesGetter ratesGetter,
            IOptions<SecondaryRatesGetterSettings> settings)
        {
            _ratesGetter = ratesGetter;
            _ratesGetter.ConfigureGetter(settings.Value);
        }

        public RatesExchangeModel GetRates() => _ratesGetter.GetRates<OpenExchangeRatesModel>();
    }
}
