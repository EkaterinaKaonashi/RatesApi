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

        public RatesGetter()
        {
            _restClient = new RestClient(_baseUrl);
            _endPoint = $"rates?key={_accessKey}&output={_outputFormat}";
        }

        public RatesOutputModel GetActualRates()
        {
            var request = new RestRequest(_endPoint, Method.GET);
            var responce = _restClient.Execute<RatesInputModel>(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
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