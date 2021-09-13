using AutoMapper;
using Exchange;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RatesApi.Constants;
using RatesApi.Settings;
using RestSharp;
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

        public void ConfigureGetter(IRatesGetterSettings settings)
        {
            _endPoint = settings.Url;
            _accessKey = settings.AccessKey;
        }

        public RatesExchangeModel GetRates<T>()
        {
            RatesExchangeModel result = default;
            var request = new RestRequest(string.Format(_endPoint, _accessKey, _settings.BaseCurrency), Method.GET);

            for (int i = 0; i < _retryCount; i++)
            {
                _logger.LogInformation(string.Format(LogMessages._requestToEndpoint, _endPoint));
                var responce = _restClient.Execute<T>(request);
                if (responce.StatusCode == HttpStatusCode.OK)
                {
                    result = Parse(responce.Data);
                    if (result == default) return result;
                    var conv = JsonConvert.SerializeObject(result);
                    _logger.LogInformation(string.Format(LogMessages._ratesWereGotten, conv));
                    return result;
                }
                var error = responce.ErrorMessage == default ? responce.Content : responce.ErrorMessage;
                _logger.LogError(string.Format(LogMessages._tryToRequestFailed, i + 1, error));
                if (i != _retryCount - 1) Thread.Sleep(_retryTimeout);
            }
            return result;
        }
        private RatesExchangeModel Parse<T>(T responseModel)
        {
            var result = _mapper.Map<RatesExchangeModel>(responseModel);
            var rates = new Dictionary<string, decimal>();
            var missingCurrencies = new List<string>();

            if (result.BaseCurrency != _settings.BaseCurrency)
            {
                _logger.LogError(string.Format(LogMessages._wrongBaseCurrecy));
                return default;
            }
            else
            {
                foreach (var currency in _settings.Currencies)
                {
                    if (result.Rates.TryGetValue(_settings.BaseCurrency + currency, out decimal rate))
                    {
                        rates.TryAdd(_settings.BaseCurrency + currency, rate);
                    }
                    else
                    {
                        missingCurrencies.Add(currency);
                    }
                }
                if (missingCurrencies.Count != 0)
                {
                    _logger.LogError(string.Format(LogMessages._currenciesWereMissed, string.Join(", ", missingCurrencies)));
                }
                result.Rates = rates;
            }
            return result;
        }
    }
}