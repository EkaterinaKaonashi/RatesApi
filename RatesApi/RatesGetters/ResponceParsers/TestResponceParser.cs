using AutoMapper;
using Exchange;
using Newtonsoft.Json;
using RatesApi.Models;
using System;
using System.Collections.Generic;

namespace RatesApi.RatesGetters.ResponceParsers
{
    public class TestResponceParser : IResponceParser
    {
        private readonly IMapper _mapper;
        private readonly string _baseCurrency;
        private readonly List<string> _currencies;
        public TestResponceParser(string baseCurrency, List<string> currencies, IMapper mapper)
        {
            _mapper = mapper;
            _baseCurrency = baseCurrency;
            _currencies = currencies;
        }

        public RatesExchangeModel Parse(string content)
        {
            var model = JsonConvert.DeserializeObject<CurrencyApiRatesModel>(content);
            foreach (var currency in model.Rates.Keys)
            {
                if (!_currencies.Contains(currency))
                {
                    model.Rates.Remove(currency);
                }
            }
            if (model.Base != _baseCurrency)
            {
                throw new Exception("Getted rates with wrong base currecy");
            }
            return _mapper.Map<RatesExchangeModel>(model);
        }
    }
}
