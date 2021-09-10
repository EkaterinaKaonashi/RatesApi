using AutoMapper;
using Exchange;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatesApi.Models;
using RatesApi.RatesGetters.ResponceParsers;
using RatesApi.Settings;
using RestSharp;
using System;
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
        private IResponseParser _responceParser;

        public RatesGetter(IOptions<CommonSettings> settings, IMapper mapper, ILogger<RatesGetter> logger)
        {
            _restClient = new RestClient();
            _settings = settings.Value;
            _logger = logger;
        }

        public void ConfigureGetter(IResponseParser parser, IRatesGetterSettings settings)
        {
            _responceParser = parser;
            _responceParser.ConfigureParser(_settings);
            _endPoint = string.Format(settings.Url, settings.AccessKey);
        }

        public RatesExchangeModel GetRates()
        {
            var result = new RatesExchangeModel();
            var request = new RestRequest(_endPoint, Method.GET);            
            var responce = _restClient.Execute<CurrencyApiRatesModel>(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {         
                return _responceParser.Parse(responce.Content);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(3600);

                    var repeatedResponse = _restClient.Execute<CurrencyApiRatesModel>(request);

                    if (repeatedResponse.StatusCode == HttpStatusCode.OK)
                    {
                        result = _responceParser.Parse(responce.Content);
                        break;
                    }

                _logger.LogError($"Responce status code: {responce.StatusCode}");

                }
            }
            return result;
        }
    }
}