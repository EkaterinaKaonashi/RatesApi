using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RatesApi.Models;
using RatesApi.RatesGetters;
using RatesApi.RatesGetters.ResponceParsers;
using RatesApi.Settings;
using System;
using System.Threading;

namespace RatesApi.Services
{
    public class SecondaryRatesService : ISecondaryRatesService
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm:ss";
        private const int _millisecondsDelay = 3600000;
        private readonly ILogger<SecondaryRatesService> _logger;
        private readonly IRatesGetter _ratesGetter;
        private readonly IMapper _mapper;

        public SecondaryRatesService(
            ILogger<SecondaryRatesService> logger,
            IMapper mapper, IRatesGetter ratesGetter,
            IOptions<SecondaryRatesGetterSettings> settings)
        {
            _logger = logger;
            _ratesGetter = ratesGetter;
            _ratesGetter.ConfigureGetter(new CurrencyApiResponceParser(mapper), settings.Value);
            _mapper = mapper;
        }

        public void GetRates()
        {
            _logger.LogInformation("RatesService running at: {time}", DateTimeOffset.Now);

            var busControl = Bus.Factory.CreateUsingRabbitMq();
            busControl.StartAsync();

            try
            {
                while (true)
                {
                    var requestTime = DateTime.Now;

                    var ratesOutput = _ratesGetter.GetRates();

                    var logModel = _mapper.Map<RatesLogModel>(ratesOutput);
                    logModel.DateTimeRequest = requestTime.ToString(_dateFormat);
                    logModel.DateTimeResponse = DateTime.Now.ToString(_dateFormat);
                    var logMessage = JsonConvert.SerializeObject(logModel);
                    _logger.LogInformation(logMessage);

                    busControl.Publish(ratesOutput);

                    Thread.Sleep(_millisecondsDelay);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }
            finally
            {
                busControl.StopAsync();
            }
        }
    }
}
