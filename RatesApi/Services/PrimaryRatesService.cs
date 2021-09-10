﻿using AutoMapper;
using Exchange;
using Microsoft.Extensions.Options;
using RatesApi.RatesGetters;
using RatesApi.RatesGetters.Deserializers;
using RatesApi.Settings;

namespace RatesApi.Services
{
    public class PrimaryRatesService : IPrimaryRatesService
    {
        private readonly IRatesGetter _ratesGetter;

        public PrimaryRatesService(
            IMapper mapper, IRatesGetter ratesGetter,
            IOptions<PrimaryRatesGetterSettings> settings)
        {
            _ratesGetter = ratesGetter;
            _ratesGetter.ConfigureGetter(new CurrencyApiResponseDeserializer(), settings.Value);
        }

        public RatesExchangeModel GetRates()
        {
            return _ratesGetter.GetRates();
        }
    }
}
