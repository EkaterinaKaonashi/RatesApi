using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RatesApi.LoggingService;
using RatesApi.Models;
using RestSharp;
using System;
using System.Net;

namespace RatesApi
{
    public class RatesGetter
    {
        private const string _baseUrl = "https://currencyapi.net/api/v1/";
        private const string _accessKey = "XVkKpQhfUmb8wWLuGwpa2IRCdky6VqIWfrwe";
        private const string _outputFormat = "JSON";
        private string _endPoint;
        private RestClient _restClient;
        private readonly ILogger<RatesGetter> _log;

        public RatesGetter(ILogger<RatesGetter> log)
        {
            _restClient = new RestClient(_baseUrl);
            _endPoint = $"rates?key={_accessKey}&output={_outputFormat}";
            _log = log;
        }

        public RatesOutputModel GetActualRates()
        {
            var info = new LogModel();

            info.DateTimeRequest = DateTime.Now;
            var request = new RestRequest(_endPoint, Method.GET);
            var responce = _restClient.Execute<RatesInputModel>(request);
            info.DateTimeResponse = DateTime.Now;
            info.CurrencyRates = responce.Data.Rates;
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                var result = JsonConvert.SerializeObject(info);
                _log.LogInformation(result);
                return new RatesOutputModel
                {
                    Updated = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddSeconds(responce.Data.Updated),
                    BaseCurrency = responce.Data.Base,
                    Rates = responce.Data.Rates
                };
            }
            else
            {
                throw new Exception($"Responce status code: {responce.StatusCode}");
            }
            
        }
    }
}