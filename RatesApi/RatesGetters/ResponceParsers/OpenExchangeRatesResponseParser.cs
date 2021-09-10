using AutoMapper;
using Exchange;
using Newtonsoft.Json;
using RatesApi.Models.InputModels;
using RatesApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatesApi.RatesGetters.ResponceParsers
{
    public class OpenExchangeRatesResponseParser
    {
        private  string _baseCurrency;
        private  List<string> _currencies;
        private readonly IMapper _mapper;

        public OpenExchangeRatesResponseParser(string baseCurrency, List<string> currencies, IMapper mapper)
        {
            _baseCurrency = baseCurrency;
            _currencies = currencies;
            _mapper = mapper;
        }
        public OpenExchangeRatesResponseParser(IMapper mapper)
        {
            _mapper = mapper;
        }
        public void ConfigureParser(CommonSettings settings)
        {
            _baseCurrency = settings.BaseCurrency;
            _currencies = settings.Currencies;
        }
        public RatesExchangeModel Parse(string content)
        {
            var model = JsonConvert.DeserializeObject<OpenExchangeRatesModel>(content);
            var currensyPairs = new Dictionary<string, decimal>();
            var missingCurrencies = new List<string>();
            foreach (var currency in _currencies)
            {
                if (!currensyPairs.TryAdd(_baseCurrency + currency, model.Rates[currency]))
                    missingCurrencies.Add(currency);
            }
            model.Rates = currensyPairs;
            if (missingCurrencies.Count != 0) throw new Exception($"The next currencies was missed: {missingCurrencies}");
            if (model.Base != _baseCurrency)
            {
                throw new Exception("Getted rates with wrong base currecy");
            }
            return _mapper.Map<RatesExchangeModel>(model);
        }
    }
}
