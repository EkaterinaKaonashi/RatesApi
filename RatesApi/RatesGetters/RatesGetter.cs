using AutoMapper;
using Exchange;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RatesApi.Constants;
using RatesApi.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace RatesApi.RatesGetters
{
    public class RatesGetter : IRatesGetter
    {
        private readonly RestClient _restClient;
        private readonly ILogger<RatesGetter> _logger;
        private readonly IMapper _mapper;
        private readonly string _baseCurrency;
        private readonly List<string> _currencies;
        private string _endPoint;
        private string _accessKey;


        public RatesGetter(IOptions<CommonSettings> settings, IMapper mapper, ILogger<RatesGetter> logger, RestClient restClient)
        {
            _restClient = restClient;
            _logger = logger;
            _baseCurrency = settings.Value.BaseCurrency;
            _currencies = settings.Value.Currencies;
            _mapper = mapper;
        }

        public void ConfigureGetter(IRatesGetterSettings settings)
        {
            _endPoint = settings.Url;
            _accessKey = settings.AccessKey;
        }

        public RatesExchangeModel GetRates<T>()
        {
            var request = new RestRequest(string.Format(_endPoint, _accessKey, _baseCurrency), Method.GET);

            _logger.LogInformation(string.Format(LogMessages._requestToEndpoint,
                string.Format(_endPoint, nameof(_accessKey), _baseCurrency)));
            var responce = _restClient.Execute<T>(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                if (TryParse(responce.Data, out RatesExchangeModel result))
                {
                    var conv = JsonConvert.SerializeObject(result);
                    _logger.LogInformation(string.Format(LogMessages._ratesWereGotten, conv));
                    return result;
                }
            }
            else
            {
                throw new Exception(responce.ErrorMessage == default ? responce.Content : responce.ErrorMessage);
            }
            return default;
        }
        private bool TryParse<T>(T responseModel, out RatesExchangeModel result)
        {
            result = _mapper.Map<RatesExchangeModel>(responseModel);
            
            if (result.BaseCurrency != _baseCurrency)
            {
                throw new Exception(LogMessages._wrongBaseCurrecy);
            }
            else
            {
                result.Rates = SeparateCurrencies(result.Rates, out List<string> missingCurrencies);
                if (missingCurrencies.Count != 0)
                {
                    throw new Exception(string.Format(LogMessages._currenciesWereMissed, string.Join(", ", missingCurrencies)));
                }
            }
            return result != default;
        }
        private Dictionary<string, decimal> SeparateCurrencies(Dictionary<string, decimal> inputRates, out List<string> missingCurrencies)
        {
            var outputRates = new Dictionary<string, decimal>();
            missingCurrencies = new List<string>();
            foreach (var currency in _currencies)
            {
                if (!inputRates.TryGetValue($"{_baseCurrency}{currency}", out decimal rate) || rate == default)
                {
                    missingCurrencies.Add(currency);
                }
                else
                {
                    outputRates.TryAdd($"{_baseCurrency}{currency}", rate);
                }
            }
            return outputRates;
        }
    }
}