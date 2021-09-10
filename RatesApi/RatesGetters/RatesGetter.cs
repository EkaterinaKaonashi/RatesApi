using AutoMapper;
using Exchange;
using Microsoft.Extensions.Options;
using RatesApi.Models;
using RatesApi.RatesGetters.ResponceParsers;
using RatesApi.Settings;
using RestSharp;
using System;
using System.Net;

namespace RatesApi.RatesGetters
{
    public class RatesGetter : IRatesGetter
    {
        private readonly RestClient _restClient;
        private readonly CommonSettings _settings;
        private string _endPoint;
        private IResponceParser _responceParser;

        public RatesGetter(IOptions<CommonSettings> settings, IMapper mapper)
        {
            _restClient = new RestClient();
            _settings = settings.Value;
        }

        public void ConfigureGetter(IResponceParser parser, IRatesGetterSettings settings)
        {
            _responceParser = parser;
            _responceParser.ConfigureParser(_settings);
            _endPoint = string.Format(settings.Url, settings.AccessKey);
        }

        public RatesExchangeModel GetRates()
        {
            var request = new RestRequest(_endPoint, Method.GET);            
            var responce = _restClient.Execute<CurrencyApiRatesModel>(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {         
                return _responceParser.Parse(responce.Content);
            }
            else
            {
                throw new Exception($"Responce status code: {responce.StatusCode}");
            }
        }
    }
}