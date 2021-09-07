using Microsoft.Extensions.Options;
using RatesApi.Models;
using RatesApi.Settings;
using RestSharp;
using System;
using System.Net;

namespace RatesApi.RatesGetter
{
    public class CurrencyApiRatesGetter : IRatesGetter
    {
        private const string _baseUrl = "https://currencyapi.net/api/v1";
        private const string _outputFormat = "JSON";
        private readonly string _accessKey;
        private readonly string _endPoint;
        private readonly RestClient _restClient;

        public CurrencyApiRatesGetter(IOptions<RatesGetterSettings> settings)
        {
            _accessKey = settings.Value.AccessKey;
            _restClient = new RestClient(_baseUrl);
            _endPoint = $"rates?key={_accessKey}&output={_outputFormat}";
        }

        public CurrencyApiRatesModel GetRates()
        {
            var request = new RestRequest(_endPoint, Method.GET);            
            var responce = _restClient.Execute<CurrencyApiRatesModel>(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                return responce.Data;               
            }
            else
            {
                throw new Exception($"Responce status code: {responce.StatusCode}");
            }
        }
    }
}