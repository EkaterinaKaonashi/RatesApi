using AutoMapper;
using Exchange;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RatesApi.Constants;
using RatesApi.Models;
using RatesApi.RatesGetters.ResponceParsers;
using RatesApi.Settings;
using RestSharp;
using System.Net;
using System.Threading;

namespace RatesApi.RatesGetters
{
    public class RatesGetter : IRatesGetter
    {
        private readonly RestClient _restClient;
        private readonly CommonSettings _settings;
        private readonly ILogger<RatesGetter> _logger;
        private string _endPoint;
        private string _accessKey;
        private IResponseParser _responceParser;
        private int _retryTimeout;
        private int _retryCount;


        public RatesGetter(IOptions<CommonSettings> settings, IMapper mapper, ILogger<RatesGetter> logger)
        {
            _restClient = new RestClient();
            _settings = settings.Value;
            _logger = logger;
            _retryTimeout = _settings.RetryTimeout;
            _retryCount = _settings.RetryCount;
        }

        public void ConfigureGetter(IResponseParser parser, IRatesGetterSettings settings)
        {
            _responceParser = parser;
            _responceParser.ConfigureParser(_settings);
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
                    result = _responceParser.Parse(responce.Content);
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
    }
}