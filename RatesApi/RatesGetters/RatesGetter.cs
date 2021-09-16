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

namespace RatesApi.RatesGetters
{
    public class RatesGetter : IRatesGetter
    {
        private readonly RestClient _restClient;
        private readonly ILogger<RatesGetter> _logger;
        private readonly IMapper _mapper;
        private string _endPoint;
        private string _accessKey;
        private string _baseCurrency;
        private List<string> _currencies;


        public RatesGetter(IOptions<CommonSettings> settings, IMapper mapper, ILogger<RatesGetter> logger)
        {
            _restClient = new RestClient();
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
                _logger.LogError(responce.ErrorMessage == default ? responce.Content : responce.ErrorMessage);
            }
            return default;
        }
        private bool TryParse<T>(T responseModel, out RatesExchangeModel result)
        {
            result = _mapper.Map<RatesExchangeModel>(responseModel);
            var rates = new Dictionary<string, decimal>();
            var missingCurrencies = new List<string>();

            if (result.BaseCurrency != _baseCurrency)
            {
                _logger.LogError(string.Format(LogMessages._wrongBaseCurrecy));
                return false;
            }
            else
            {
                foreach (var currency in _currencies)
                {
                    if (!result.Rates.TryGetValue($"{_baseCurrency}{currency}", out decimal rate) || rate == default)
                    {
                        missingCurrencies.Add(currency);
                    }
                    else
                    {
                        rates.TryAdd($"{_baseCurrency}{currency}", rate);
                    }
                }
                result.Rates = rates;
                if (missingCurrencies.Count != 0)
                {
                    _logger.LogError(string.Format(LogMessages._currenciesWereMissed, string.Join(", ", missingCurrencies)));
                    return false;
                }
            }
            return result != default;
        }
    }
}