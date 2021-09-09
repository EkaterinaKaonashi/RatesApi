using AutoMapper;
using Exchange;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RatesApi.Models;
using RatesApi.RatesGetters;
using RatesApi.RatesGetters.ResponceParsers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RatesApi
{
    public class RatesService : IRatesService
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm:ss";
        private const int _millisecondsDelay = 3600000;
        private readonly ILogger<RatesService> _logger;
        private readonly IRatesGetter _ratesGetter;
        private readonly IMapper _mapper;
        private readonly List<IResponceParser> _parsers;

        public RatesService(ILogger<RatesService> logger, IMapper mapper, IRatesGetter ratesGetter)
        {
            _logger = logger;
            _ratesGetter = ratesGetter;
            _mapper = mapper;
        }

        public void Run()
        {
            _logger.LogInformation("RatesService running at: {time}", DateTimeOffset.Now);

            var busControl = Bus.Factory.CreateUsingRabbitMq();
            busControl.StartAsync();

            try
            {
                while (true)
                {
                    var requestTime = DateTime.Now;

                    var ratesOutput = _mapper.Map<RatesExchangeModel>(_ratesGetter.GetRates());

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
