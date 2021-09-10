using AutoMapper;
using Exchange;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RatesApi.Constants;
using RatesApi.Models;
using RatesApi.RatesGetters.Deserializers;
using RatesApi.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace RatesApi.RatesGetters
{
    public class RatesGetter : IRatesGetter
    {
        private readonly RestClient _restClient;
        private readonly CommonSettings _settings;
        private readonly ILogger<RatesGetter> _logger;
        private readonly IMapper _mapper;
        private string _endPoint;
        private string _accessKey;
        private IDeserializer _deserializer;
        private int _retryTimeout;
        private int _retryCount;


        public RatesGetter(IOptions<CommonSettings> settings, IMapper mapper, ILogger<RatesGetter> logger)
        {
            _restClient = new RestClient();
            _settings = settings.Value;
            _logger = logger;
            _retryTimeout = _settings.RetryTimeout;
            _retryCount = _settings.RetryCount;
            _mapper = mapper;
        }

        public void ConfigureGetter(IDeserializer deserializer, IRatesGetterSettings settings)
        {
            _deserializer = deserializer;
            _endPoint = settings.Url;
            _accessKey = settings.AccessKey;
        }

        public RatesExchangeModel GetRates()
        {
            RatesExchangeModel result = default;
            var request = new RestRequest(string.Format(_endPoint, _accessKey), Method.GET);
            IRestResponse responce = default;

            for (int i = 0; i < _retryCount; i++)
            {
                _logger.LogInformation(string.Format(LogMessages._requestToEndpoint, _endPoint));
                responce = _restClient.Execute<CurrencyApiRatesModel>(request);

                if (responce.StatusCode == HttpStatusCode.OK)
                {
                    result = Parse(responce.Content);
                    var conv = JsonConvert.SerializeObject(result);
                    _logger.LogInformation(string.Format(LogMessages._ratesWereGotten, conv));
                    return result;
                }
                _logger.LogInformation(string.Format(LogMessages._tryToRequestFailed, i+1));
                if (i != _retryCount - 1) Thread.Sleep(_retryTimeout);
            }
            _logger.LogError(string.Format(LogMessages._responceStatusCode, responce.StatusCode));
            return result;
        }
        private RatesExchangeModel Parse(string content)
        {
            var model = _deserializer.Deserialize(content);
            var currensyPairs = new Dictionary<string, decimal>();
            var missingCurrencies = new List<string>();
            foreach (var currency in _settings.Currencies)
            {
                if (!currensyPairs.TryAdd(_settings.BaseCurrency + currency, model.Rates[currency]))
                    missingCurrencies.Add(currency);
            }
            model.Rates = currensyPairs;
            if (missingCurrencies.Count != 0) throw new Exception($"The next currencies was missed: {missingCurrencies}");
            if (model.Base != _settings.BaseCurrency)
            {
                throw new Exception("Getted rates with wrong base currecy");
            }
            return _mapper.Map<RatesExchangeModel>(model);
        }
    }
}