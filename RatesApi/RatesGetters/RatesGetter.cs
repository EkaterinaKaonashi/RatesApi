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
        private readonly string _endPoint;
        private readonly RestClient _restClient;
        private readonly CurrencyApiResponceParser _responceParser;
        private readonly IMapper _mapper;

        public RatesGetter(IOptions<RatesGetterSettings> settings, IMapper mapper)
        {
            _restClient = new RestClient();
            _endPoint = string.Format(settings.Value.PrimaryUrl, settings.Value.PrimaryAccessKey);
            _mapper = mapper;
            _responceParser = new CurrencyApiResponceParser(settings.Value.BaseCurrency, settings.Value.Currencies, mapper);
            var d = _responceParser.GetType().FullName;
            Type type = Type.GetType(d);

            object instance = Activator.CreateInstance(type, settings.Value.BaseCurrency, settings.Value.Currencies, mapper);
        }

        public RatesExchangeModel GetRates()
        {
            var request = new RestRequest(_endPoint, Method.GET);            
            var responce = _restClient.Execute<CurrencyApiRatesModel>(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                //return responce.Data;               
                return _responceParser.Parse(responce.Content);
            }
            else
            {
                throw new Exception($"Responce status code: {responce.StatusCode}");
            }
        }
    }
}