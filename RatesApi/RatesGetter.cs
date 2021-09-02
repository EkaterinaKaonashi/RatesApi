using RatesApi.Models;
using RestSharp;
using System;

namespace RatesApi
{
    public class RatesGetter
    {
        private const string _baseUrl = "http://api.currencylayer.com/";
        private const string _accessKey = "3600ce33721acfddd94f550f8a9b445c";
        private const string _currencies = "USD,RUB,EUR,JPY";
        private const int _format = 1;
        private string _endPoint = $"live?access_key=3600ce33721acfddd94f550f8a9b445c&currencies=USD,RUB,EUR,JPY&format=1";
        private RestClient _restClient;

        public RatesGetter()
        {
            _restClient = new RestClient(_baseUrl);
            _endPoint = $"live?access_key={_accessKey}&currencies={_currencies}&format={_format}";
        }

        public RatesOutputModel GetActualRates()
        {
            var request = new RestRequest(_endPoint, Method.GET);
            //var responce = _restClient.Execute<RatesInputModel>(request);
            return new RatesOutputModel
            {
                DateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local)
                //DateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddSeconds(responce.Data.Timestamp),
                //Source = responce.Data.Source,
                //Quotes = responce.Data.Quotes
            };
        }
    }
}